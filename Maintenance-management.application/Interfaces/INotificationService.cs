namespace Maintenance_management.application.Interfaces;

public interface INotificationService
{
    /// <summary>Sends a real-time notification to a specific user.</summary>
    Task SendToUserAsync(string userId, string title, string message,
        string type = "info", string? relatedEntityId = null, string? relatedEntityType = null);

    /// <summary>Sends a real-time notification to all users in a given role.</summary>
    Task SendToRoleAsync(string role, string title, string message,
        string type = "info", string? relatedEntityId = null, string? relatedEntityType = null);
}
