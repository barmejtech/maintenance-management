using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IBankReconciliationRepository : IRepository<BankReconciliation>
{
    Task<BankReconciliation?> GetWithEntriesAsync(Guid id);
    Task<IEnumerable<BankReconciliation>> GetAllWithEntriesAsync();
    Task<ReconciliationEntry> AddEntryAsync(ReconciliationEntry entry);
    Task<IEnumerable<ReconciliationEntry>> GetEntriesByReconciliationIdAsync(Guid reconciliationId);
}