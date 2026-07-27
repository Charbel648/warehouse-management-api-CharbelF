using Warehouse.Notifications.Infrastructure.Messaging;
using Warehouse.Notifications.Api.Messaging;
using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Infrastructure;
using Warehouse.Notifications.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNotificationsInfrastructure(builder.Configuration);


builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<RabbitMqWarehouseEventsConsumer>();
var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();




