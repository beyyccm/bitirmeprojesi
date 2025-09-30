using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UcakRezervasyon.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TimeAccessFilterAttribute : Attribute, IActionFilter
{
    private readonly int _fromHour;
    private readonly int _toHour;

    public TimeAccessFilterAttribute(int fromHour, int toHour)
    {
        _fromHour = fromHour;
        _toHour = toHour;
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var utcNow = DateTime.UtcNow;
        var hour = utcNow.Hour;
        if (!(_fromHour <= hour && hour < _toHour))
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Content = $"This endpoint is accessible only between {_fromHour}:00 and {_toHour}:00 UTC."
            };
        }
    }
}
