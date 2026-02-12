using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using SapGateway.Models;
using SapGateway.Services;
using Serilog;
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
                if (await sl.IsApInvoiceDraftExists(company, body.U_PDG_Voucher_No))
                {
                    throw new Exception("Voucher already used.");
                }


                InvoiceDraftModel invoiceDraft = new InvoiceDraftModel();
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
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
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
