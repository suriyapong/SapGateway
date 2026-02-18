using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using SapGateway.Models;
using SapGateway.Services;
using Serilog;
using System.Reflection;
using System.Text;
using static SapGateway.Models.SAPPurchaseRequestModel;

namespace SapGateway.Endpoints
{
    public static class BASSnetSAPEndpoints
    {
        public static void MapApInvoiceBotEndpoints(this WebApplication app)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MY_SUPER_SECRET_KEY_123456"));
            var group = app.MapGroup("/apinvoicebot/{company}");

            group.MapPost("/create", HandleCreateApInvoice);

            Log.Information("AP Invoice Bot endpoints registered");
        }

        private static async Task<IResult> HandleCreateApInvoice(string company, APInvoiceServiceModel body, SapServiceLayerClient sl, SeqLogService seqLog)
        {
            try
            {
                if (body == null) return Results.BadRequest(new { message = "Request body is required" });

                //Step Validation
                if (!await sl.IsVendorPaymentExists(company, body.CardCode))
                {
                    throw new InvalidOperationException("Vendor does not exist in SAP.");
                }

                foreach (var line in body.DocumentLines)
                {
                    // ✅ Check Account Code
                    if (!await sl.AccountExistsAsync(company, line.AccountCode))
                    {
                        throw new InvalidOperationException(
                            $"Account code '{line.AccountCode}' does not exist in SAP."
                        );
                    }

                    // ✅ Check Vat Group
                    if (!string.IsNullOrWhiteSpace(line.VatGroup) &&
                        !await sl.VatGroupExistsAsync(company, line.VatGroup))
                    {
                        throw new InvalidOperationException(
                            $"VAT Group '{line.VatGroup}' does not exist in SAP."
                        );
                    }
                }

                InvoiceDraftModel invoiceDraft = new InvoiceDraftModel();
                body.DocCurrency = MapToSapCurrency(body.DocCurrency);

                invoiceDraft = await sl.InsertAPInvoiceService(company, body);

                seqLog.LogInfo("Currency update executed successfully", new
                {
                    Company = company,
                    Document = "A/P Invoice Service",
                    RequestBy = "Bot",
                    Timestamp = DateTime.UtcNow
                });

                return Results.Ok(new { Message = $"Create A/P Invoice company : { company }", Data = invoiceDraft });
            }
            catch (Exception ex)
            {
                //Detail อยากให้ Return Text
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

 

        public static string MapToSapCurrency(string bassnetCurrency)
        {
            Dictionary<string, string> _currencyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // ===== USD =====
                { "USD", "USS" },
                { "US$", "USS" },
                { "US DOLLAR", "USS" },
                { "U.S. DOLLAR", "USS" },
                { "840", "USS" },

                // ===== EUR =====
                { "EUR", "EUS" },
                { "EURO", "EUS" },
                { "978", "EUS" },

                // ===== GBP =====
                { "GBP", "GBP" },
                { "POUND", "GBP" },
                { "826", "GBP" },

                // ===== SGD =====
                { "SGD", "SGS" },
                { "SINGAPORE DOLLAR", "SGS" },
                { "702", "SGS" },

                // ===== MYR =====
                { "MYR", "MYS" },
                { "MALAYSIAN RINGGIT", "MYS" },
                { "458", "MYS" },

                // ===== THB =====
                { "THB", "THB" },
                { "THAI BAHT", "THB" },
                { "764", "THB" },

                // ===== JPY =====
                { "JPY", "JPY" },
                { "JAPANESE YEN", "JPY" },
                { "392", "JPY" },

                // ===== CNY =====
                { "CNY", "CNY" },
                { "RMB", "CNY" },
                { "156", "CNY" },

                // ===== HKD =====
                { "HKD", "HKD" },
                { "344", "HKD" },

                // ===== AUD =====
                { "AUD", "AUD" },
                { "036", "AUD" },

                // ===== NZD =====
                { "NZD", "NZD" },
                { "554", "NZD" },

                // ===== CHF =====
                { "CHF", "CHF" },
                { "756", "CHF" },

                // ===== CAD =====
                { "CAD", "CAD" },
                { "124", "CAD" },

                // ===== IDR =====
                { "IDR", "IDR" },
                { "360", "IDR" },

                // ===== VND =====
                { "VND", "VND" },
                { "704", "VND" },

                // ===== PHP =====
                { "PHP", "PHP" },
                { "608", "PHP" },

                // ===== KRW =====
                { "KRW", "KRW" },
                { "410", "KRW" },

                // ===== INR =====
                { "INR", "INR" },
                { "356", "INR" },

                // ===== ZAR =====
                { "ZAR", "ZAR" },
                { "710", "ZAR" },

                // ===== AED =====
                { "AED", "AED" },
                { "784", "AED" },

                // ===== SAR =====
                { "SAR", "SAR" },
                { "682", "SAR" },

                // ===== QAR =====
                { "QAR", "QAR" },
                { "634", "QAR" }
            };

            if (string.IsNullOrWhiteSpace(bassnetCurrency))
                throw new ArgumentException("Currency is required.");

                var normalized = bassnetCurrency.Trim().ToUpper();

                if (_currencyMap.TryGetValue(normalized, out var sapCurrency))
                    return sapCurrency;

                    throw new KeyNotFoundException($"Currency '{bassnetCurrency}' is not mapped for SAP."
            );
        }
    }

    public class APInvoiceServiceModel
    {
        //Key Vendor Code + Invoice Vendor
        public string CardCode { get; set; }
        public DateTime DocDate { get; set; }
        public string DocType { get; set; } //dDocument_Service หรือ dDocument_Items ถ้าไม่ใส่จะ Default dDocument_Items, 
        public string Comments { get; set; }
        public string DocCurrency { get; set; } //ใส่ SAP ต้องมีอัตราแลกเปลี่ยนนนี้ด้วย
        public string NumAtCard { get; set; } //_ExtDocumentNo
        public string U_PDG_PO_BASSnet { get; set; } //_PONo
        public string? DocObjectCode { get; set; } //ถ้าสร้าง Draft ต้องใส่ "oPurchaseInvoices"
        // ===== Lines =====
        public List<DocumentLines> DocumentLines { get; set; } = new List<DocumentLines>();
    }


    public class DocumentLines
    {
        public string AccountCode { get; set; }
        public double UnitPrice { get; set; }
        public string VatGroup { get; set; } //"PNV", //TAX Code
        public string ItemDescription { get; set; }
    }
}
