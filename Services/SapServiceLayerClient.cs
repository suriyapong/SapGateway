using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using SapGateway.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Text;
using Azure.Core;
using System.Xml.Linq;
using SapGateway.Endpoints;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.WebRequestMethods;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Net.Http;
using System;
using System.Globalization;

namespace SapGateway.Services
{
    public class SapServiceLayerClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly SapSessionCache _cache;
            
        public SapServiceLayerClient(HttpClient http, IConfiguration config, SapSessionCache cache)
        {
            _http = http;
            _config = config;
            _cache = cache;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _http = new HttpClient(handler); // �� handler ������ SSL

            _http.BaseAddress = new Uri(_config["SapServiceLayer:BaseUrl"]);
        }

        private async Task Login(string company)
        {
            try
            {
                var companyDB = _config.GetSection($"SapDbMap:{company}").Value;
                Console.WriteLine(companyDB);

                var body = new
                {
                    CompanyDB = companyDB,
                    UserName = _config["SapServiceLayer:Username"],
                    Password = _config["SapServiceLayer:Password"]
                };

                string jsonData = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var res = await _http.PostAsync("Login", content);
                string result = await res.Content.ReadAsStringAsync();

                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                string sessionId = json.GetProperty("SessionId").GetString()!;
                string routeId = res.Headers.GetValues("Set-Cookie")
                    .First(x => x.Contains("ROUTEID"))
                    .Split("=")[1].Split(";")[0];

                _http.DefaultRequestHeaders.Add("Cookie", $"B1SESSION={sessionId}; ROUTEID={routeId}");

                _cache.Set(company, new SapSession
                {
                    SessionId = sessionId,
                    RouteId = routeId,
                    ExpireAt = DateTime.UtcNow.AddMinutes(15)
                });
                Console.WriteLine("Login success");
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }

        }

        private async Task EnsureLogin(string company)
        {
            if (_cache.TryGet(company, out var session))
            {
                _http.DefaultRequestHeaders.Remove("Cookie");
                _http.DefaultRequestHeaders.Add("Cookie", $"B1SESSION={session.SessionId}; ROUTEID={session.RouteId}");

                return;
            }
            await Login(company);
        }

        public async Task InsertCurrencyData(string company, string sapCurrencyId, string rateDate, double rate)
        {
            await EnsureLogin(company);

            var jsonExchangeRate = new Dictionary<string, object>
            {
                { "Currency", sapCurrencyId },
                { "Rate", rate },
                { "RateDate", rateDate},
            };

            string jsonData = JsonConvert.SerializeObject(jsonExchangeRate);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
               var response = await _http.PostAsync("SBOBobService_SetCurrencyRate", content);

                using (response)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Update Curency success");
                    }
                    else
                    {
                        var respText = await response.Content.ReadAsStringAsync();
                        throw new Exception($"SAP update currency failed for company {company} ({sapCurrencyId}). " + $"Response: {response.StatusCode} - {respText}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InsertCurrencyData] Error: {ex.Message}", ex);
                // ✅ โยนต่อขึ้นไปพร้อมบอกชื่อ service/function
                throw new Exception($"[InsertCurrencyData] Error: {ex.Message}", ex);
            }
        }

        public async Task<InvoiceDraftModel> InsertAPInvoiceService(string company, APInvoiceServiceModel request)
        {
            InvoiceDraftModel invoiceDraft = new InvoiceDraftModel();

            await EnsureLogin(company);

            string jsonData = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("Drafts", content);
  
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Insert A/P Invoice Success");
                response.EnsureSuccessStatusCode();       
                invoiceDraft = await response.Content.ReadFromJsonAsync<InvoiceDraftModel>();    
            }
            else
            {
                var respText = await response.Content.ReadAsStringAsync();
                var sapMessage = ExtractSapMessage(respText);

                throw new Exception($"SAP Draft creation failed: {sapMessage}");
            }

            return invoiceDraft;
        }


        public async Task<bool> IsVendorPaymentExists(string company, string cardCode)
        {
            await EnsureLogin(company);

            var filter = Uri.EscapeDataString($"CardCode eq '{cardCode}'");
            var response = await _http.GetAsync($"BusinessPartners?$select=CardCode&$filter={filter}&$top=1");

            response.EnsureSuccessStatusCode();

            using var doc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var value = doc.RootElement.GetProperty("value");

            if (value.GetArrayLength() == 0)
                return false;

            var sapCardCode = value[0].GetProperty("CardCode").GetString();

            return string.Equals(sapCardCode, cardCode, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> AccountExistsAsync(string company, string accountCode)
        {
            await EnsureLogin(company);

            var filter = Uri.EscapeDataString($"Code eq '{accountCode}'");

            var res = await _http.GetAsync($"ChartOfAccounts?$select=Code&$filter={filter}&$top=1");
            res.EnsureSuccessStatusCode();

            using var doc = System.Text.Json.JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("value").GetArrayLength() > 0;
        }

        public async Task<bool> VatGroupExistsAsync(string company, string vatGroupCode)
        {
            await EnsureLogin(company);

            var filter = Uri.EscapeDataString($"Code eq '{vatGroupCode}'");

            var res = await _http.GetAsync($"VatGroups?$select=Code&$filter={filter}&$top=1");
            res.EnsureSuccessStatusCode();

            using var doc = System.Text.Json.JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("value").GetArrayLength() > 0;
        }


        public async Task<string> GetPO(string company, string? poNum)
        {
            await EnsureLogin(company);
            string filter = string.IsNullOrEmpty(poNum) ? "" : $"?$filter=DocNum eq {poNum}";
            var res = await _http.GetAsync($"PurchaseOrders{filter}");

            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _cache.Remove(company);
                await Login(company);
                res = await _http.GetAsync($"PurchaseOrders{filter}");
            }

            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync();
        }

        private static string ExtractSapMessage(string respText)
        {
            try
            {
                using var doc = JsonDocument.Parse(respText);

                var error = doc.RootElement.GetProperty("error");
                var messageElement = error.GetProperty("message");

                // กรณี message เป็น object (Production case)
                if (messageElement.ValueKind == JsonValueKind.Object)
                {
                    if (messageElement.TryGetProperty("value", out var valueProp))
                    {
                        return valueProp.GetString() ?? respText;
                    }
                }

                // กรณี message เป็น string (บาง environment)
                if (messageElement.ValueKind == JsonValueKind.String)
                {
                    var message = messageElement.GetString();

                    // เผื่อมันเป็น JSON string ซ้อนอีกชั้น
                    if (!string.IsNullOrWhiteSpace(message) &&
                        message.TrimStart().StartsWith("{"))
                    {
                        using var inner = JsonDocument.Parse(message);

                        return inner.RootElement
                            .GetProperty("error")
                            .GetProperty("message")
                            .GetProperty("value")
                            .GetString() ?? message;
                    }

                    return message ?? respText;
                }

                return respText;
            }
            catch
            {
                return respText;
            }
        }

    }
}
