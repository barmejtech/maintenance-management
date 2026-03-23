using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IChatMessageRepository : IRepository<ChatMessage>
{
    /// <summary>Returns the last <paramref name="count"/> public messages, oldest-first.</summary>
    Task<IEnumerable<ChatMessage>> GetHistoryAsync(int count = 100);

    /// <summary>
    /// Returns the conversation between two users (private DMs) plus public messages,
    /// newest-first, limited to <paramref name="count"/> records.
    /// </summary>
    Task<IEnumerable<ChatMessage>> GetHistoryForUserAsync(string userId, int count = 100);
}
