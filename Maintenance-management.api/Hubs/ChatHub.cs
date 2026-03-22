using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Maintenance_management.api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    /// <summary>
    /// Called when a client connects. Announces the user's arrival to all clients.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var userName = Context.User?.Identity?.Name ?? "Unknown";

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "global-chat");
            await Clients.Others.SendAsync("UserConnected", userId, userName);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        var userName = Context.User?.Identity?.Name ?? "Unknown";

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "global-chat");
            await Clients.Others.SendAsync("UserDisconnected", userId, userName);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Broadcasts a message to all connected clients.
    /// </summary>
    public async Task SendMessage(string message)
    {
        var userId = Context.UserIdentifier ?? "unknown";
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        var timestamp = DateTime.UtcNow;

        await Clients.Group("global-chat").SendAsync("ReceiveMessage", userId, userName, message, timestamp);
    }

    /// <summary>
    /// Sends a private message to a specific user.
    /// </summary>
    public async Task SendPrivateMessage(string recipientUserId, string message)
    {
        var senderId = Context.UserIdentifier ?? "unknown";
        var senderName = Context.User?.Identity?.Name ?? "Unknown";
        var timestamp = DateTime.UtcNow;

        // Send to the recipient
        await Clients.User(recipientUserId).SendAsync("ReceivePrivateMessage", senderId, senderName, message, timestamp);
        // Echo back to sender
        await Clients.Caller.SendAsync("ReceivePrivateMessage", senderId, senderName, message, timestamp);
    }
}
