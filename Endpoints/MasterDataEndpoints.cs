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
    public static class MasterDataEndpoints
    {
        public static void MapMasterDataEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/masterdata/{company}");

            group.MapGet("/getalluom", HandleAllUOM);
            Log.Information("Purchase Request Bot endpoints registered");
        }

        private static async Task<IResult> HandleAllUOM(string company, SapServiceLayerClient sl, SeqLogService seqLog)
        {
            try
            {
                SAPUOMModel list = new SAPUOMModel();
                list = await sl.GetAllUnitOfMeasurements(company);
                return Results.Ok(new { Message = $"Unit Of Measurements : {company}", data = list });
            }
            catch (Exception ex)
            {
                //Detail อยากให้ Return Text
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
