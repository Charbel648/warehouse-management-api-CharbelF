using Warehouse.Notifications.Domain.Models;

namespace Warehouse.Notifications.Application.Interfaces;

public interface INotificationRepository
{
    Task<List<NotificationRecord>> GetAllAsync(
        string? type,
        string? severity,
        string? status,
        CancellationToken cancellationToken);

    Task<NotificationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsByEventIdAsync(string eventId, CancellationToken cancellationToken);

    Task AddAsync(NotificationRecord notification, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}


