namespace Warehouse.Presentation.Responses;

public class ApiErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string TraceId { get; set; } = string.Empty;

    public IDictionary<string, string[]>? ValidationErrors { get; set; }
}
