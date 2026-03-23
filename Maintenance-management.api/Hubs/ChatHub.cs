using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Maintenance_management.api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatMessageRepository _chatRepo;

    public ChatHub(IChatMessageRepository chatRepo)
    {
        _chatRepo = chatRepo;
    }

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
    /// Broadcasts a text message to all connected clients and persists it to DB.
    /// </summary>
    public async Task SendMessage(string message)
    {
        var userId = Context.UserIdentifier ?? "unknown";
        var userName = Context.User?.Identity?.Name ?? "Unknown";

        var entity = new ChatMessage
        {
            SenderId = userId,
            SenderName = userName,
            Content = message,
            MessageType = MessageType.Text
        };

        var saved = await _chatRepo.AddAsync(entity);

        var dto = MapToDto(saved);
        await Clients.Group("global-chat").SendAsync("ReceiveMessage", dto);
    }

    /// <summary>
    /// Broadcasts a file/photo message to all connected clients and persists it to DB.
    /// </summary>
    public async Task SendFileMessage(string fileUrl, string fileName, string contentType, bool isPhoto)
    {
        var userId = Context.UserIdentifier ?? "unknown";
        var userName = Context.User?.Identity?.Name ?? "Unknown";

        var entity = new ChatMessage
        {
            SenderId = userId,
            SenderName = userName,
            Content = fileName,
            MessageType = isPhoto ? MessageType.Photo : MessageType.File,
            FileUrl = fileUrl,
            FileName = fileName,
            ContentType = contentType
        };

        var saved = await _chatRepo.AddAsync(entity);

        var dto = MapToDto(saved);
        await Clients.Group("global-chat").SendAsync("ReceiveMessage", dto);
    }

    private static object MapToDto(ChatMessage m) => new
    {
        id = m.Id,
        senderId = m.SenderId,
        senderName = m.SenderName,
        content = m.Content,
        messageType = (int)m.MessageType,
        fileUrl = m.FileUrl,
        fileName = m.FileName,
        contentType = m.ContentType,
        sentAt = m.CreatedAt
    };
}

