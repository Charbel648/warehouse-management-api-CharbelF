using Warehouse.Notifications.Application.Contracts;
using Warehouse.Notifications.Domain.Events;

namespace Warehouse.Notifications.Application.Interfaces;

public interface INotificationService
{
    Task HandleWarehouseEventAsync(WarehouseNotificationEvent warehouseEvent, CancellationToken cancellationToken);

    Task<List<NotificationDto>> GetNotificationsAsync(
        string? type,
        string? severity,
        string? status,
        CancellationToken cancellationToken);

    Task<NotificationDto?> MarkAsReadAsync(Guid id, CancellationToken cancellationToken);
}


