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

            group.MapGet("/list-budget", async (string year, string company, string department, string budgetCode, SapSqlConnect sql) =>
                    await HandleGetBudgetsMock(year, company, department, budgetCode, sql))
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

        private static async Task<IResult> HandleGetBudgetsMock(string year, string company, string? department,string? budgetCode, SapSqlConnect sql)
        {
            try
            {
                int yearFilter = int.Parse(year);

                // Mock Data FY24 + FY25
                var data = new List<BudgetModel>
                {
                    // FY25
                    new BudgetModel { Year = 2025, Department = "IT",  BudgetCode = "BG-001", BudgetAmount = 1200000, ActualAmount = 950000 },
                    new BudgetModel { Year = 2025, Department = "IT",  BudgetCode = "BG-002", BudgetAmount = 800000,  ActualAmount = 820000 },
                    new BudgetModel { Year = 2025, Department = "HR",  BudgetCode = "BG-101", BudgetAmount = 400000, ActualAmount = 300000 },
                    new BudgetModel { Year = 2025, Department = "HR",  BudgetCode = "BG-102", BudgetAmount = 500000, ActualAmount = 600000 },
                    new BudgetModel { Year = 2025, Department = "FIN", BudgetCode = "BG-201", BudgetAmount = 900000, ActualAmount = 700000 },

                    // FY24
                    new BudgetModel { Year = 2024, Department = "IT",  BudgetCode = "BG-001", BudgetAmount = 1100000, ActualAmount = 900000 },
                    new BudgetModel { Year = 2024, Department = "IT",  BudgetCode = "BG-002", BudgetAmount = 780000,  ActualAmount = 760000 },
                    new BudgetModel { Year = 2024, Department = "HR",  BudgetCode = "BG-101", BudgetAmount = 380000, ActualAmount = 350000 },
                    new BudgetModel { Year = 2024, Department = "FIN", BudgetCode = "BG-201", BudgetAmount = 850000, ActualAmount = 840000 }
                };

                // Calculate UnderOver
                data.ForEach(x => x.UnderOver = x.BudgetAmount - x.ActualAmount);

                // Filter by year (บังคับต้องส่ง)
                data = data.Where(x => x.Year == yearFilter).ToList();

                // Filter Department
                if (!string.IsNullOrWhiteSpace(department))
                    data = data
                        .Where(x => x.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                // Filter Budget Code
                if (!string.IsNullOrWhiteSpace(budgetCode))
                    data = data
                        .Where(x => x.BudgetCode.Equals(budgetCode, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                // Summary
                var summary = new
                {
                    year = yearFilter,
                    count = data.Count,
                    totalBudget = data.Sum(x => x.BudgetAmount),
                    totalActual = data.Sum(x => x.ActualAmount),
                    totalUnderOver = data.Sum(x => x.UnderOver)
                };

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
