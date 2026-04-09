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
    public static class PurchaseRequestEndpoints
    {
        public static void MapPurchaseRequestEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/purchaserequest/{company}");

            group.MapPost("/createitem", HandleCreatePurchaseRequestIteme);
            group.MapPost("/createservice", HandleCreatePurchaseService);
            group.MapPost("/converttopr", HandleConvertToPurchaseRequest);

            Log.Information("Purchase Request Bot endpoints registered");
        }

        private static async Task<IResult> HandleCreatePurchaseRequestIteme(string company, SAPPurchaseRequestModel request, SapServiceLayerClient sl, SeqLogService seqLog)
        {
            try
            {
                PurchaseRequestDraftModel pr = new PurchaseRequestDraftModel();
                pr = await sl.InsertPurchaseRequest(company, request);
                return Results.Ok(new { Message = $"Create Purchase request item company : {company}", data = pr });
            }
            catch (Exception ex)
            {
                //Detail อยากให้ Return Text
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> HandleCreatePurchaseService(string company, SAPPurchaseRequestModel request, SapServiceLayerClient sl, SeqLogService seqLog)
        {
            try
            {
                PurchaseRequestDraftModel pr = new PurchaseRequestDraftModel();
                pr = await sl.InsertPurchaseRequest(company, request);
                return Results.Ok(new { Message = $"Create Purchase request service company : {company}", data = pr });
            }
            catch (Exception ex)
            {
                //Detail อยากให้ Return Text
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> HandleConvertToPurchaseRequest(string company, DocumentRequestModel docEntry, SapServiceLayerClient sl, SeqLogService seqLog)
        {
            try
            {
                PurchaseRequestDraftModel pr = new PurchaseRequestDraftModel();
                bool result = await sl.ConvertToPurchaseRequest(company, docEntry);
 
                return Results.Ok(new { Message = $"Purchase Request created successfully for company : {company}" });
            }
            catch (Exception ex)
            {
                //Detail อยากให้ Return Text
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

    }
}
