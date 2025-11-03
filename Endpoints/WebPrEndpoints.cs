using SapGateway.Services;

namespace SapGateway.Endpoints
{
    public static class WebPrEndpoints
    {
        public static void MapWebPrEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/webpr/{company}");

            group.MapPost("/create-pr", async (string company, SapServiceLayerClient sl) =>
            {
                return Results.Ok(new { Message = $"PR created for {company}" });
            });
        }
    }
}
