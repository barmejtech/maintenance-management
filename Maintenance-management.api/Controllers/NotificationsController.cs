using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repo;

    public NotificationsController(INotificationRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Returns this user's notifications from the last 5 days.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var since = DateTime.UtcNow.AddDays(-5);
        var notifications = await _repo.GetForUserAsync(userId, since);
        return Ok(notifications.Select(MapToDto));
    }

    /// <summary>Marks a single notification as read.</summary>
    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = GetUserId();
        await _repo.MarkAsReadAsync(id, userId);
        return NoContent();
    }

    /// <summary>Marks all of this user's notifications as read.</summary>
    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        await _repo.MarkAllAsReadAsync(userId);
        return NoContent();
    }

    /// <summary>Soft-deletes a notification.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var notification = await _repo.GetByIdAsync(id);
        if (notification is null || notification.UserId != userId)
            return NotFound();
        await _repo.DeleteAsync(id);
        return NoContent();
    }

    private string GetUserId()
        => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    private static object MapToDto(AppNotification n) => new
    {
        id = n.Id,
        title = n.Title,
        message = n.Message,
        type = (int)n.Type,
        isRead = n.IsRead,
        createdAt = n.CreatedAt,
        relatedEntityId = n.RelatedEntityId,
        relatedEntityType = n.RelatedEntityType
    };
}
