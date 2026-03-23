using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatMessageRepository _chatRepo;

    public ChatController(IChatMessageRepository chatRepo)
    {
        _chatRepo = chatRepo;
    }

    /// <summary>Returns the last 100 chat messages ordered oldest-first.</summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int count = 100)
    {
        var messages = await _chatRepo.GetHistoryAsync(count);
        var result = messages.Select(m => new
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
        });
        return Ok(result);
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
