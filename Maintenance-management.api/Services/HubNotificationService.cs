using Maintenance_management.api.Hubs;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Maintenance_management.api.Services;

/// <summary>
/// Persists notifications to the database and sends real-time updates
/// through the SignalR <see cref="NotificationHub"/>.
/// </summary>
public class HubNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationRepository _notificationRepo;

    public HubNotificationService(
        IHubContext<NotificationHub> hub,
        UserManager<ApplicationUser> userManager,
        INotificationRepository notificationRepo)
    {
        _hub = hub;
        _userManager = userManager;
        _notificationRepo = notificationRepo;
    }

    public async Task SendToUserAsync(string userId, string title, string message,
        string type = "info", string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var notification = await PersistAsync(userId, title, message, type, relatedEntityId, relatedEntityType);
        var payload = MapToPayload(notification);
        await _hub.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToRoleAsync(string role, string title, string message,
        string type = "info", string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        var tasks = users.Select(async u =>
        {
            var notification = await PersistAsync(u.Id, title, message, type, relatedEntityId, relatedEntityType);
            var payload = MapToPayload(notification);
            await _hub.Clients.Group($"user-{u.Id}").SendAsync("ReceiveNotification", payload);
        });
        await Task.WhenAll(tasks);
    }

    private async Task<AppNotification> PersistAsync(string userId, string title, string message,
        string type, string? relatedEntityId, string? relatedEntityType)
    {
        var entity = new AppNotification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = MapType(type),
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType
        };
        return await _notificationRepo.AddAsync(entity);
    }

    private static object MapToPayload(AppNotification n) => new
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

    private static NotificationType MapType(string type) => type.ToLowerInvariant() switch
    {
        "success" => NotificationType.Success,
        "warning" => NotificationType.Warning,
        "error"   => NotificationType.Error,
        _         => NotificationType.Info
    };
}
