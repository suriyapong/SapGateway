using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using SapGateway.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Azure.Core;
using System.Xml.Linq;

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
            // if (_cache.TryGet(company, out var session))
            // {
            //     _http.DefaultRequestHeaders.Remove("Cookie");
            //     _http.DefaultRequestHeaders.Add("Cookie", $"B1SESSION={session.SessionId}; ROUTEID={session.RouteId}");

            //     return;
            // }
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
                        Console.WriteLine("Sap update curency fail");
                        throw new Exception("Sap update curency fail");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding exchangerate: " + ex.Message);
            }
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
    }
}
