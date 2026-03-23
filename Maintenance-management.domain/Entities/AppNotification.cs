using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class AppNotification : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; } = false;
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}
