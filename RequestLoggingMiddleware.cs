using System.Security.Claims;

namespace UcakRezervasyon.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var url = context.Request.Path;
        var time = DateTime.UtcNow;
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        _logger.LogInformation("Request: {Url} at {Time} by {User}", url, time, userId);
        await _next(context);
    }
}
