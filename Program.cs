using SapGateway.Services;
using SapGateway.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<SapSessionCache>();
builder.Services.AddSingleton<SapServiceLayerClient>();
builder.Services.AddSingleton<SapSqlConnect>();
builder.Services.AddHttpClient<SapServiceLayerClient>();

var app = builder.Build();

// Map endpoints
app.MapPowerBiEndpoints();
app.MapWebPrEndpoints();
app.MapCurencyBotEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
