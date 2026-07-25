namespace Warehouse.Notifications.Application.Contracts;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string EventId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RelatedEntityId { get; set; } = string.Empty;
    public string RelatedEntityType { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
}
