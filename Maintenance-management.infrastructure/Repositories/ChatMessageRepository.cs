using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class ChatMessageRepository : Repository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ChatMessage>> GetHistoryAsync(int count = 100)
    {
        return await _context.ChatMessages
            .Where(m => !m.IsDeleted && m.RecipientId == null)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetPrivateHistoryAsync(string userId1, string userId2, int count = 100)
    {
        return await _context.ChatMessages
            .Where(m => !m.IsDeleted &&
                        ((m.SenderId == userId1 && m.RecipientId == userId2) ||
                         (m.SenderId == userId2 && m.RecipientId == userId1)))
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}
