using Warehouse.Notifications.Application.Contracts;
using Warehouse.Notifications.Domain.Events;
using Warehouse.Notifications.Application.Interfaces;
using Warehouse.Notifications.Domain.Models;

namespace Warehouse.Notifications.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task HandleWarehouseEventAsync(
        WarehouseNotificationEvent warehouseEvent,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(warehouseEvent.EventId))
        {
            return;
        }

        bool alreadyProcessed = await _notificationRepository.ExistsByEventIdAsync(
            warehouseEvent.EventId,
            cancellationToken);

        if (alreadyProcessed)
        {
            return;
        }

        NotificationRecord notification = CreateNotificationFromEvent(warehouseEvent);

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(
        string? type,
        string? severity,
        string? status,
        CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetAllAsync(
            type,
            severity,
            status,
            cancellationToken);

        return notifications.Select(ToDto).ToList();
    }

    public async Task<NotificationDto?> MarkAsReadAsync(Guid id, CancellationToken cancellationToken)
    {
        NotificationRecord? notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);

        if (notification == null)
        {
            return null;
        }

        notification.MarkAsRead();

        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return ToDto(notification);
    }

    private static NotificationRecord CreateNotificationFromEvent(WarehouseNotificationEvent warehouseEvent)
    {
        if (warehouseEvent.EventType == "StockLowDetected")
        {
            string productName = string.IsNullOrWhiteSpace(warehouseEvent.ProductName)
                ? warehouseEvent.RelatedEntityId
                : warehouseEvent.ProductName;

            string message =
                $"Product '{productName}' is low in stock. Current quantity: {warehouseEvent.Quantity}. Threshold: {warehouseEvent.Threshold}.";

            return new NotificationRecord(
                warehouseEvent.EventId,
                "LowStock",
                "Low stock detected",
                message,
                warehouseEvent.Severity,
                warehouseEvent.RelatedEntityId,
                warehouseEvent.RelatedEntityType);
        }

        if (warehouseEvent.EventType == "WarehouseFileUploaded")
        {
            string fileName = string.IsNullOrWhiteSpace(warehouseEvent.FileName)
                ? "A warehouse file"
                : warehouseEvent.FileName;

            string message =
                $"{fileName} was uploaded for {warehouseEvent.RelatedEntityType} with ID {warehouseEvent.RelatedEntityId}.";

            return new NotificationRecord(
                warehouseEvent.EventId,
                "FileUploaded",
                "Warehouse file uploaded",
                message,
                warehouseEvent.Severity,
                warehouseEvent.RelatedEntityId,
                warehouseEvent.RelatedEntityType);
        }

        return new NotificationRecord(
            warehouseEvent.EventId,
            "WarehouseEvent",
            "Warehouse event received",
            $"Warehouse event '{warehouseEvent.EventType}' was received.",
            warehouseEvent.Severity,
            warehouseEvent.RelatedEntityId,
            warehouseEvent.RelatedEntityType);
    }

    private static NotificationDto ToDto(NotificationRecord notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            EventId = notification.EventId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            Severity = notification.Severity,
            Status = notification.Status,
            RelatedEntityId = notification.RelatedEntityId,
            RelatedEntityType = notification.RelatedEntityType,
            CreatedAtUtc = notification.CreatedAtUtc,
            ReadAtUtc = notification.ReadAtUtc
        };
    }
}


