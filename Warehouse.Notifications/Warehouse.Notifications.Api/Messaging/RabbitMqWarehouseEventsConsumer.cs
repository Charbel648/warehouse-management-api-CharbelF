using Warehouse.Notifications.Infrastructure.Messaging;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Warehouse.Notifications.Domain.Events;
using Warehouse.Notifications.Application.Interfaces;

namespace Warehouse.Notifications.Api.Messaging;

public class RabbitMqWarehouseEventsConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqWarehouseEventsConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqWarehouseEventsConsumer(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqWarehouseEventsConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                StartConsumer();

                _logger.LogInformation(
                    "RabbitMQ notification consumer started. Exchange: {ExchangeName}, Queue: {QueueName}",
                    _options.ExchangeName,
                    _options.QueueName);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "RabbitMQ is not available yet. Notification service will retry in 5 seconds.");

                DisposeRabbitMq();

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private void StartConsumer()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        _channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: "stock.low");

        _channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: "file.uploaded");

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += HandleMessageAsync;

        _channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer);
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        if (_channel == null)
        {
            return;
        }

        try
        {
            string json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            WarehouseNotificationEvent? warehouseEvent = JsonSerializer.Deserialize<WarehouseNotificationEvent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (warehouseEvent == null)
            {
                _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                return;
            }

            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            await notificationService.HandleWarehouseEventAsync(
                warehouseEvent,
                CancellationToken.None);

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to process RabbitMQ warehouse event. The message will not be requeued.");

            _channel.BasicNack(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: false);
        }
    }

    public override void Dispose()
    {
        DisposeRabbitMq();
        base.Dispose();
    }

    private void DisposeRabbitMq()
    {
        try
        {
            _channel?.Close();
            _connection?.Close();
        }
        catch
        {
            // Ignore shutdown errors.
        }

        _channel?.Dispose();
        _connection?.Dispose();

        _channel = null;
        _connection = null;
    }
}




