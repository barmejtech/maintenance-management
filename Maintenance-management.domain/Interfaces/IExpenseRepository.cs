using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<Expense?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Expense>> GetAllWithDetailsAsync();
    Task<IEnumerable<Expense>> GetByVendorIdAsync(Guid vendorId);
    Task<IEnumerable<Expense>> GetByStatusAsync(ExpenseStatus status);
    Task<IEnumerable<Expense>> GetByRenovationIdAsync(Guid renovationId);
}