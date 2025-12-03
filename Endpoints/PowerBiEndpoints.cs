using SapGateway.Services;
using Microsoft.Data.SqlClient;
using SapGateway.Models;
using System.Text.Json;

namespace SapGateway.Endpoints
{
    public static class PowerBiEndpoints
    {
        public static void MapPowerBiEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/powerbi/{company}");

            // Service Layer endpoints
            group.MapGet("/list-po", async (string company, SapServiceLayerClient sl) =>
                await HandleGetPurchaseOrders(company, sl))
                .WithName("GetPurchaseOrders")
                .WithTags("PowerBi");

            // SQL-based endpoints
            group.MapGet("/list-po-sql", async (string company, SapSqlConnect sql) =>
                await HandleGetPurchaseOrdersSql(company, sql))
                .WithName("GetPurchaseOrdersSql")
                .WithTags("PowerBi");

            group.MapGet("/list-invoice-sql", async (string company, SapSqlConnect sql) =>
                await HandleGetInvoicesSql(company, sql))
                .WithName("GetInvoicesSql")
                .WithTags("PowerBi");

            group.MapGet("/list-budget", async (string company, string department, string budgetCode, SapSqlConnect sql) =>
                    await HandleGetBudgetsMock(company, department, budgetCode, sql))
                .WithName("GetBudget")
                .WithTags("PowerBi");
        }

        // -----------------------------
        // Service Layer Handlers
        // -----------------------------
        private static async Task<IResult> HandleGetPurchaseOrders(string company, SapServiceLayerClient sl)
        {
            try
            {
                var jsonData = await sl.GetPO(company, null);
                return Results.Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // -----------------------------
        // SQL Handlers
        // -----------------------------
        private static async Task<IResult> HandleGetPurchaseOrdersSql(string company, SapSqlConnect sql)
        {
            try
            {
                var query = @"
                    SELECT TOP 50 DocEntry, DocNum, CardCode, CardName, DocDate, DocTotal
                    FROM OPOR
                    ORDER BY DocDate DESC";

                var result = await sql.QueryAsync(company, query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> HandleGetInvoicesSql(string company, SapSqlConnect sql)
        {
            try
            {
                var query = @"
                    SELECT TOP 50 DocEntry, DocNum, CardCode, CardName, DocDate, DocTotal
                    FROM OINV
                    ORDER BY DocDate DESC";

                var result = await sql.QueryAsync(company, query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> HandleGetBudgetsMock(
            string company,
            string? department,
            string? budgetCode,
            SapSqlConnect sql)
        {
            try
            {
                // Mock data
                var data = new List<BudgetModel>
        {
            new BudgetModel { Department = "IT", BudgetCode = "BG-001", BudgetFY25 = 1200000, ActualFY25 = 950000 },
            new BudgetModel { Department = "IT", BudgetCode = "BG-002", BudgetFY25 = 800000, ActualFY25 = 820000 },
            new BudgetModel { Department = "HR", BudgetCode = "BG-101", BudgetFY25 = 400000, ActualFY25 = 300000 },
            new BudgetModel { Department = "HR", BudgetCode = "BG-102", BudgetFY25 = 500000, ActualFY25 = 600000 },
            new BudgetModel { Department = "FIN", BudgetCode = "BG-201", BudgetFY25 = 900000, ActualFY25 = 700000 }
        };

                // Calculate UnderOver
                data.ForEach(x => x.UnderOver = x.BudgetFY25 - x.ActualFY25);

                // Filters
                if (!string.IsNullOrWhiteSpace(department))
                    data = data.Where(x =>
                        x.Department.Equals(department, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                if (!string.IsNullOrWhiteSpace(budgetCode))
                    data = data.Where(x =>
                        x.BudgetCode.Equals(budgetCode, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                // Meta summary
                var summary = new
                {
                    count = data.Count,
                    totalBudget = data.Sum(x => x.BudgetFY25),
                    totalActual = data.Sum(x => x.ActualFY25),
                    totalUnderOver = data.Sum(x => x.UnderOver)
                };

                // Serialize
                var jsonData = JsonSerializer.Serialize(new { data, summary });

                return Results.Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
