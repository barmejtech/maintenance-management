using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface INotificationRepository : IRepository<AppNotification>
{
    Task<IEnumerable<AppNotification>> GetForUserAsync(string userId, DateTime since);
    Task MarkAsReadAsync(Guid id, string userId);
    Task MarkAllAsReadAsync(string userId);
}
