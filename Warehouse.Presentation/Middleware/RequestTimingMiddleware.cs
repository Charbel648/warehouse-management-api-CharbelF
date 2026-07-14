using System.Diagnostics;

namespace Warehouse.Presentation.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(
        RequestDelegate next,
        ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
            return Task.CompletedTask;
        });

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            context.TraceIdentifier);
    }
}
