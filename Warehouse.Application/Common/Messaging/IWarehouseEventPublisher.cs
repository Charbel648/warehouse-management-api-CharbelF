namespace Warehouse.Application.Common.Messaging;

public interface IWarehouseEventPublisher
{
    Task PublishAsync(
        WarehouseNotificationEvent warehouseEvent,
        string routingKey,
        CancellationToken cancellationToken);
}

