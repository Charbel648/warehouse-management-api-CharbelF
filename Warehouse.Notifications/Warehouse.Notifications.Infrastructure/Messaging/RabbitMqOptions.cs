namespace Warehouse.Notifications.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "warehouse";
    public string Password { get; set; } = "warehouse";
    public string ExchangeName { get; set; } = "warehouse.events";
    public string QueueName { get; set; } = "notifications.warehouse-events";
}
