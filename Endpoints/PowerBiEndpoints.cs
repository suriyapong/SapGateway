using SapGateway.Services;
using Microsoft.Data.SqlClient;

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
    }
}
