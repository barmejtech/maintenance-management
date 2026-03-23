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
            .Where(m => !m.IsDeleted && m.ReceiverId == null)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetHistoryForUserAsync(string userId, int count = 100)
    {
        return await _context.ChatMessages
            .Where(m => !m.IsDeleted &&
                        (m.ReceiverId == null ||                          // public
                         m.SenderId == userId ||                          // sent by user
                         m.ReceiverId == userId))                         // received by user
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}
