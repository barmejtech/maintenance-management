using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IChatMessageRepository : IRepository<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> GetHistoryAsync(int count = 100);
    Task<IEnumerable<ChatMessage>> GetPrivateHistoryAsync(string userId1, string userId2, int count = 100);
}
