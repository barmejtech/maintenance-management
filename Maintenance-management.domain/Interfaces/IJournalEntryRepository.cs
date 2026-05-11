using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IJournalEntryRepository : IRepository<JournalEntry>
{
    Task<JournalEntry?> GetWithLineItemsAsync(Guid id);
    Task<IEnumerable<JournalEntry>> GetAllWithLineItemsAsync();
    Task<IEnumerable<JournalEntry>> GetPostedEntriesUpToDateAsync(DateTime date);
    Task<IEnumerable<JournalEntry>> GetPostedEntriesByDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime start, DateTime end);
}