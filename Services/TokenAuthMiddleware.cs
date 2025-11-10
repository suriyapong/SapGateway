using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class TokenAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public TokenAuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var configuredToken = _config["AppSettings:ApiToken"];

        // ดึงค่า Token จาก Header
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Missing Authorization header" });
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        if (token != configuredToken)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid token" });
            return;
        }

        await _next(context); // ผ่านได้ -> เรียก endpoint ต่อ
    }
}