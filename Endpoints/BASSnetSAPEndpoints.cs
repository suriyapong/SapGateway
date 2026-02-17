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

                    // ===== EUR =====
                    { "EUR", "EUS" },
                    { "EURO", "EUS" },

                    // ===== SGD =====
                    { "SGD", "SGS" },

                    // ===== MYR =====
                    { "MYR", "MYS" },

                    // ===== GBP =====
                    { "GBP", "GBP" }
                };

                if (string.IsNullOrWhiteSpace(bassnetCurrency))
                    throw new ArgumentException("Currency is required.");

                    var normalized = bassnetCurrency.Trim().ToUpper();

                    if (_currencyMap.TryGetValue(normalized, out var sapCurrency))
                        return sapCurrency;

                    throw new KeyNotFoundException(
                        $"Currency '{bassnetCurrency}' is not mapped for SAP."
                );
            }
    }

    public class APInvoiceServiceModel
    {
        //Key Vendor Code + Invoice
        // ===== Header =====
        public string U_PDG_Voucher_No { get; set; } //Key
        public string CardCode { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime RequriedDate { get; set; }
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
