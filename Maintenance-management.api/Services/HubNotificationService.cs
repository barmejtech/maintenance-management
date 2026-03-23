using Maintenance_management.api.Hubs;
using Maintenance_management.application.Interfaces;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Maintenance_management.api.Services;

/// <summary>
/// Sends real-time notifications through the SignalR <see cref="NotificationHub"/>.
/// </summary>
public class HubNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;
    private readonly UserManager<ApplicationUser> _userManager;

    public HubNotificationService(
        IHubContext<NotificationHub> hub,
        UserManager<ApplicationUser> userManager)
    {
        _hub = hub;
        _userManager = userManager;
    }

    public async Task SendToUserAsync(string userId, string title, string message,
        string type = "info", string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var payload = BuildPayload(title, message, type, relatedEntityId, relatedEntityType);
        await _hub.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToRoleAsync(string role, string title, string message,
        string type = "info", string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        var payload = BuildPayload(title, message, type, relatedEntityId, relatedEntityType);

        var tasks = users.Select(u =>
            _hub.Clients.Group($"user-{u.Id}").SendAsync("ReceiveNotification", payload));

        await Task.WhenAll(tasks);
    }

    private static object BuildPayload(string title, string message, string type,
        string? relatedEntityId, string? relatedEntityType) => new
        {
            id = Guid.NewGuid().ToString(),
            title,
            message,
            type = MapType(type),
            isRead = false,
            createdAt = DateTime.UtcNow,
            relatedEntityId,
            relatedEntityType
        };

    private static int MapType(string type) => type.ToLowerInvariant() switch
    {
        "success" => 1,
        "warning" => 2,
        "error"   => 3,
        _         => 0   // info
    };
}
