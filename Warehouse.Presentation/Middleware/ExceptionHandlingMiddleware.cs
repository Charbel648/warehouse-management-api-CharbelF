using System.ComponentModel.DataAnnotations;
using Warehouse.Domain.Exceptions;
using Warehouse.Presentation.Responses;

namespace Warehouse.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
                throw;

            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                context.TraceIdentifier);

            await WriteErrorResponseAsync(context, exception);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var errorCode = "unexpected_error";
        var message = "An unexpected server error occurred";

        switch (exception)
        {
            case NotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                errorCode = "not_found";
                message = exception.Message;
                break;

            case BusinessRuleException businessRuleException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = businessRuleException.ErrorCode;
                message = businessRuleException.Message;
                break;

            case ValidationException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "validation_error";
                message = exception.Message;
                break;

            case ArgumentException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "invalid_request";
                message = exception.Message;
                break;

            case InvalidOperationException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "business_rule_violation";
                message = exception.Message;
                break;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            ErrorCode = errorCode,
            Message = message,
            TraceId = context.TraceIdentifier
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
