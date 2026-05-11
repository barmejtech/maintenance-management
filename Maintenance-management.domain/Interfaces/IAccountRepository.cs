using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetWithChildrenAsync(Guid id);
    Task<IEnumerable<Account>> GetByTypeAsync(Enums.AccountType type);
    Task<decimal> GetBalanceAsync(Guid id);
    Task<bool> CodeExistsAsync(string accountCode, Guid? excludeId = null);
}