namespace Warehouse.Notifications.Domain.Models;

public class NotificationRecord
{
    public Guid Id { get; private set; }
    public string EventId { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string Severity { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Unread";
    public string RelatedEntityId { get; private set; } = string.Empty;
    public string RelatedEntityType { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ReadAtUtc { get; private set; }

    private NotificationRecord()
    {
    }

    public NotificationRecord(
        string eventId,
        string type,
        string title,
        string message,
        string severity,
        string relatedEntityId,
        string relatedEntityType)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        Type = type;
        Title = title;
        Message = message;
        Severity = severity;
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType;
        Status = "Unread";
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        if (Status == "Read")
        {
            return;
        }

        Status = "Read";
        ReadAtUtc = DateTime.UtcNow;
    }
}


