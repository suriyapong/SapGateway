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
    public static class PurchaseOrderEndpoints
    {
        public static void MapPurchaseOrderEndpoints(this WebApplication app)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MY_SUPER_SECRET_KEY_123456"));
            var group = app.MapGroup("/purchaseorder/{company}");

            group.MapGet("/get", HandleGetPurchaseOrder);

            Log.Information("Purchaseorder Bot endpoints registered");
        }

        private static async Task<IResult> HandleGetPurchaseOrder(string company, int docNum, SapServiceLayerClient sl, SeqLogService seqLog)
        {
            try
            {
                if (docNum == null) return Results.BadRequest(new { message = "Request PONo is required" });

                SAPResponse<SAPPurchaseOrderModel> po = new SAPResponse<SAPPurchaseOrderModel>();
                po = await sl.GetPurchaseOrderByDocNum(company, docNum);

                return Results.Ok(new { Message = $"Get Purchase order company : {company}", data = po.value});
            }
            catch (Exception ex)
            {
                //Detail อยากให้ Return Text
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
