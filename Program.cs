using SapGateway.Services;
using SapGateway.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.Seq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SeqLogService>(); // สมมติคุณใช้ SeqLogService อยู่แล้ว

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Seq(
        serverUrl: "http://172.20.45.7:5341/"
    )
    .CreateLogger();
// Configure Serilog


// ================= ADD SERVICES =================
builder.Services.AddSingleton<SapSessionCache>();
builder.Services.AddSingleton<SapServiceLayerClient>();
builder.Services.AddSingleton<SapSqlConnect>();
builder.Services.AddHttpClient<SapServiceLayerClient>();
builder.Services.AddSingleton<SeqLogService>();

var app = builder.Build();

// ================= USE MIDDLEWARE =================
app.UseMiddleware<TokenAuthMiddleware>(); // ? ตรวจ token ก่อนถึง endpoint

// Map endpoints
app.MapPowerBiEndpoints();
app.MapWebPrEndpoints();
app.MapCurencyBotEndpoints();

app.MapGet("/", () => "SAP Gateway API Running...");

app.Run();
