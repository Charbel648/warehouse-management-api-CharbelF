using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Warehouse.Presentation.Filters;

public class ActionLoggingFilter : IActionFilter
{
    private const string StopwatchKey = "ActionStopwatch";
    private readonly ILogger<ActionLoggingFilter> _logger;

    public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items[StopwatchKey] = Stopwatch.StartNew();

        _logger.LogInformation(
            "Executing action {Action}. TraceId: {TraceId}",
            context.ActionDescriptor.DisplayName,
            context.HttpContext.TraceIdentifier);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Items[StopwatchKey] is Stopwatch stopwatch)
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "Executed action {Action} in {ElapsedMilliseconds}ms. TraceId: {TraceId}",
                context.ActionDescriptor.DisplayName,
                stopwatch.ElapsedMilliseconds,
                context.HttpContext.TraceIdentifier);
        }
    }
}
