using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class ChatMessage : BaseEntity
{
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    /// <summary>Null means a public broadcast message; non-null means a private DM to the specified user.</summary>
    public string? ReceiverId { get; set; }
    public string? Content { get; set; }
    public MessageType MessageType { get; set; } = MessageType.Text;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}
