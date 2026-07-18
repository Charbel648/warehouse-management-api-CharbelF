using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;
using Warehouse.Domain.Exceptions;
using Warehouse.Presentation.Resources;
using Warehouse.Presentation.Responses;

namespace Warehouse.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IStringLocalizer<SharedResources> localizer)
    {
        _next = next;
        _logger = logger;
        _localizer = localizer;
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

    private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var errorCode = "unexpected_error";
        var message = _localizer["UnexpectedError"].Value;

        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                errorCode = "not_found";
                message = _localizer[
                    "ResourceNotFound",
                    notFoundException.ResourceName,
                    notFoundException.ResourceId].Value;
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
