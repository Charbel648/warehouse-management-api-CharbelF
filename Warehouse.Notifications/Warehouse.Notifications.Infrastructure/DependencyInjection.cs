using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Notifications.Application.Interfaces;
using Warehouse.Notifications.Application.Services;
using Warehouse.Notifications.Infrastructure.Messaging;
using Warehouse.Notifications.Infrastructure.Persistence;
using Warehouse.Notifications.Infrastructure.Repositories;

namespace Warehouse.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("NotificationsDb")
            ?? "Data Source=notifications.db";

        services.AddDbContext<NotificationDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddHostedService<RabbitMqWarehouseEventsConsumer>();

        return services;
    }
}
