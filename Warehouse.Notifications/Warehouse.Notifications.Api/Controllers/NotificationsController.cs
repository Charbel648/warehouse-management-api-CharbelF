using Microsoft.AspNetCore.Mvc;
using Warehouse.Notifications.Application.Contracts;
using Warehouse.Notifications.Application.Interfaces;

namespace Warehouse.Notifications.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications(
        [FromQuery] string? type,
        [FromQuery] string? severity,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        List<NotificationDto> notifications = await _notificationService.GetNotificationsAsync(
            type,
            severity,
            status,
            cancellationToken);

        return Ok(notifications);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<NotificationDto>> MarkAsRead(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        NotificationDto? notification = await _notificationService.MarkAsReadAsync(
            id,
            cancellationToken);

        if (notification == null)
        {
            return NotFound(new
            {
                message = "Notification was not found."
            });
        }

        return Ok(notification);
    }
}
