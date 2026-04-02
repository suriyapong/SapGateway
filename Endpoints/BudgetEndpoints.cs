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
    public static class WebPrEndpoints
    {
        public static void MapBudgetEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/budget/{company}");

            group.MapGet("/getAllBudget", HandleGetAllBudget);
            group.MapGet("/getAllBudgetDetail", HandleGetAllBudgetDetail);
            Log.Information("Budget Bot endpoints registered");
        }

        private static async Task<IResult> HandleGetAllBudget(string company, int year, SapSqlConnect sc, SeqLogService seqLog)
        {
            List<SAPBudgetModel> budget = await sc.GetAllBudget(company, year);

            return Results.Ok(new { Message = $"Get all budget company : {company}", Data = budget });
        }

        private static async Task<IResult> HandleGetAllBudgetDetail(string company, int year, SapSqlConnect sc, SeqLogService seqLog)
        {
            List<SAPBudgetDetailModel> budget = await sc.GetAllBudgetDetail(company, year);

            return Results.Ok(new { Message = $"Get all budget detail company : {company}", Data = budget });
        }
    }
}