using Warehouse.Application.Common.Messaging;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class NoOpWarehouseEventPublisher : IWarehouseEventPublisher
{
    public Task PublishAsync(
        WarehouseNotificationEvent warehouseEvent,
        string routingKey,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
