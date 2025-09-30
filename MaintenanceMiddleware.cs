using UcakRezervasyon.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace UcakRezervasyon.Api.Middlewares;

public class MaintenanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MaintenanceMiddleware> _logger;
    public MaintenanceMiddleware(RequestDelegate next, ILogger<MaintenanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        try
        {
            var m = await db.Maintenances.FirstOrDefaultAsync();
            if (m != null && m.IsInMaintenance)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new { message = m.Message ?? "Service is under maintenance" });
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not check maintenance status");
        }

        await _next(context);
    }
}
