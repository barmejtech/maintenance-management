using Maintenance_management.api.Hubs;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatMessageRepository _chatRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(IChatMessageRepository chatRepo, UserManager<ApplicationUser> userManager)
    {
        _chatRepo = chatRepo;
        _userManager = userManager;
    }

    /// <summary>Returns the last 100 public chat messages ordered oldest-first.</summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int count = 100)
    {
        var messages = await _chatRepo.GetHistoryAsync(count);
        var result = messages.Select(m => new
        {
            id = m.Id,
            senderId = m.SenderId,
            senderName = m.SenderName,
            recipientId = m.RecipientId,
            content = m.Content,
            messageType = (int)m.MessageType,
            fileUrl = m.FileUrl,
            fileName = m.FileName,
            contentType = m.ContentType,
            sentAt = m.CreatedAt
        });
        return Ok(result);
    }

    /// <summary>Returns the last 100 private messages between the current user and another user.</summary>
    [HttpGet("private-history/{otherUserId}")]
    public async Task<IActionResult> GetPrivateHistory(string otherUserId, [FromQuery] int count = 100)
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var messages = await _chatRepo.GetPrivateHistoryAsync(currentUserId, otherUserId, count);
        var result = messages.Select(m => new
        {
            id = m.Id,
            senderId = m.SenderId,
            senderName = m.SenderName,
            recipientId = m.RecipientId,
            content = m.Content,
            messageType = (int)m.MessageType,
            fileUrl = m.FileUrl,
            fileName = m.FileName,
            contentType = m.ContentType,
            sentAt = m.CreatedAt
        });
        return Ok(result);
    }

    /// <summary>Returns all users with their online/offline status.</summary>
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var onlineIds = ChatHub.GetOnlineUserIds().ToHashSet();
        var users = _userManager.Users
            .Select(u => new
            {
                id = u.Id,
                fullName = u.FirstName + " " + u.LastName,
                email = u.Email ?? string.Empty,
                isOnline = onlineIds.Contains(u.Id)
            })
            .OrderBy(u => u.fullName)
            .ToList();
        return Ok(users);
    }

    /// <summary>Persist a text message via REST (fallback when hub is unavailable).</summary>
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequest dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content) && string.IsNullOrWhiteSpace(dto.FileUrl))
            return BadRequest(new { message = "Content or file is required." });

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var userName = User.Identity?.Name ?? "Unknown";

        var entity = new ChatMessage
        {
            SenderId = userId,
            SenderName = userName,
            Content = dto.Content,
            MessageType = dto.MessageType,
            FileUrl = dto.FileUrl,
            FileName = dto.FileName,
            ContentType = dto.ContentType
        };

        var saved = await _chatRepo.AddAsync(entity);

        return Ok(new
        {
            id = saved.Id,
            senderId = saved.SenderId,
            senderName = saved.SenderName,
            recipientId = saved.RecipientId,
            content = saved.Content,
            messageType = (int)saved.MessageType,
            fileUrl = saved.FileUrl,
            fileName = saved.FileName,
            contentType = saved.ContentType,
            sentAt = saved.CreatedAt
        });
    }
}

public record SendChatMessageRequest(
    string? Content,
    MessageType MessageType = MessageType.Text,
    string? FileUrl = null,
    string? FileName = null,
    string? ContentType = null
);
