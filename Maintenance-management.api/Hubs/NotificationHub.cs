using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Maintenance_management.api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    /// <summary>
    /// Called when a client connects. Adds the user to their personal group
    /// so that server-side code can push notifications to a specific user.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Client calls this to mark a notification as read.
    /// </summary>
    public async Task MarkAsRead(string notificationId)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Group($"user-{userId}").SendAsync("NotificationRead", notificationId);
        }
    }
}
