using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Warehouse.Application.Common.Messaging;

namespace Warehouse.Infrastructure.Messaging;

public class RabbitMqWarehouseEventPublisher : IWarehouseEventPublisher
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqWarehouseEventPublisher> _logger;

    public RabbitMqWarehouseEventPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqWarehouseEventPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync(
        WarehouseNotificationEvent warehouseEvent,
        string routingKey,
        CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            string json = JsonSerializer.Serialize(warehouseEvent);

            byte[] body = Encoding.UTF8.GetBytes(json);

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = warehouseEvent.EventId;
            properties.CorrelationId = warehouseEvent.CorrelationId;

            channel.BasicPublish(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.LogInformation(
                "Published warehouse event {EventType} with routing key {RoutingKey}",
                warehouseEvent.EventType,
                routingKey);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to publish warehouse event {EventType}. The warehouse operation will continue.",
                warehouseEvent.EventType);
        }

        return Task.CompletedTask;
    }
}

