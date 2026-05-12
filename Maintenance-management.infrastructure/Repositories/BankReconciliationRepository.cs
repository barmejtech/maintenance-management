using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class BankReconciliationRepository : Repository<BankReconciliation>, IBankReconciliationRepository
{
    public BankReconciliationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<BankReconciliation?> GetWithEntriesAsync(Guid id)
        => await _dbSet
            .Include(b => b.Entries)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

    public async Task<IEnumerable<BankReconciliation>> GetAllWithEntriesAsync()
        => await _dbSet
            .Include(b => b.Entries)
            .Where(b => !b.IsDeleted)
            .OrderByDescending(b => b.StatementDate)
            .ToListAsync();

    public async Task<ReconciliationEntry> AddEntryAsync(ReconciliationEntry entry)
    {
        _context.Set<ReconciliationEntry>().Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<IEnumerable<ReconciliationEntry>> GetEntriesByReconciliationIdAsync(Guid reconciliationId)
        => await _context.Set<ReconciliationEntry>()
            .Where(e => e.BankReconciliationId == reconciliationId)
            .OrderBy(e => e.TransactionDate)
            .ToListAsync();
}
