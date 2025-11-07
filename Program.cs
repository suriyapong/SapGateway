using SapGateway.Services;
using SapGateway.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<SapSessionCache>();
builder.Services.AddSingleton<SapServiceLayerClient>();
builder.Services.AddSingleton<SapSqlConnect>();
builder.Services.AddHttpClient<SapServiceLayerClient>();

var app = builder.Build();



// --- Use Middleware ---
app.UseAuthentication();
app.UseAuthorization();



// Map endpoints
app.MapPowerBiEndpoints();
app.MapWebPrEndpoints();
app.MapCurencyBotEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
