using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.Presentation.Responses;

namespace Warehouse.Presentation.Filters;

public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        var errors = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => item.Key,
                item => item.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Invalid value"
                        : error.ErrorMessage)
                    .ToArray());

        context.Result = new BadRequestObjectResult(new ApiErrorResponse
        {
            ErrorCode = "validation_error",
            Message = "One or more validation errors occurred",
            TraceId = context.HttpContext.TraceIdentifier,
            ValidationErrors = errors
        });
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
