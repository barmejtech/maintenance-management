using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IChatMessageRepository : IRepository<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> GetHistoryAsync(int count = 100);
}
