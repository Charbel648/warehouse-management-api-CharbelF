using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Application.Interfaces;
using Warehouse.Notifications.Domain.Models;
using Warehouse.Notifications.Infrastructure.Persistence;

namespace Warehouse.Notifications.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _dbContext;

    public NotificationRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<NotificationRecord>> GetAllAsync(
        string? type,
        string? severity,
        string? status,
        CancellationToken cancellationToken)
    {
        IQueryable<NotificationRecord> query = _dbContext.Notifications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(notification => notification.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = query.Where(notification => notification.Severity == severity);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(notification => notification.Status == status);
        }

        return await query
            .OrderByDescending(notification => notification.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<NotificationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Notifications
            .FirstOrDefaultAsync(notification => notification.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByEventIdAsync(string eventId, CancellationToken cancellationToken)
    {
        return _dbContext.Notifications
            .AnyAsync(notification => notification.EventId == eventId, cancellationToken);
    }

    public async Task AddAsync(NotificationRecord notification, CancellationToken cancellationToken)
    {
        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}


