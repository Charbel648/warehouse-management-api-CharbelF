namespace Warehouse.Application.Common.Messaging;

public class WarehouseNotificationEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime EventTimeUtc { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string RelatedEntityId { get; set; } = string.Empty;
    public string RelatedEntityType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Information";

    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? Quantity { get; set; }
    public int? Threshold { get; set; }

    public string? FileId { get; set; }
    public string? FileName { get; set; }
    public string? FileCategory { get; set; }
    public string? UploadedByFirebaseUid { get; set; }
}

