using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class JournalEntryRepository : Repository<JournalEntry>, IJournalEntryRepository
{
    public JournalEntryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<JournalEntry?> GetWithLineItemsAsync(Guid id)
        => await _dbSet
            .Include(j => j.LineItems)
                .ThenInclude(li => li.Account)
            .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);

    public async Task<IEnumerable<JournalEntry>> GetAllWithLineItemsAsync()
        => await _dbSet
            .Include(j => j.LineItems)
                .ThenInclude(li => li.Account)
            .Where(j => !j.IsDeleted)
            .OrderByDescending(j => j.EntryDate)
            .ToListAsync();

    public async Task<IEnumerable<JournalEntry>> GetPostedEntriesUpToDateAsync(DateTime date)
        => await _dbSet
            .Include(j => j.LineItems)
                .ThenInclude(li => li.Account)
            .Where(j => j.IsPosted && j.EntryDate <= date && !j.IsDeleted)
            .OrderBy(j => j.EntryDate)
            .ToListAsync();

    public async Task<IEnumerable<JournalEntry>> GetPostedEntriesByDateRangeAsync(DateTime start, DateTime end)
        => await _dbSet
            .Include(j => j.LineItems)
                .ThenInclude(li => li.Account)
            .Where(j => j.IsPosted && j.EntryDate >= start && j.EntryDate <= end && !j.IsDeleted)
            .OrderBy(j => j.EntryDate)
            .ToListAsync();

    public async Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime start, DateTime end)
        => await _dbSet
            .Include(j => j.LineItems)
                .ThenInclude(li => li.Account)
            .Where(j => j.EntryDate >= start && j.EntryDate <= end && !j.IsDeleted)
            .OrderBy(j => j.EntryDate)
            .ToListAsync();
}
