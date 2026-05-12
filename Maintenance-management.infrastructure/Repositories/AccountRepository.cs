using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Account?> GetWithChildrenAsync(Guid id)
        => await _dbSet
            .Include(a => a.ParentAccount)
            .Include(a => a.ChildAccounts)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

    public async Task<IEnumerable<Account>> GetByTypeAsync(domain.Enums.AccountType type)
    {
        // Both AccountType enums have identical values, safe to cast
        var entityType = (AccountType)(int)type;
        return await _dbSet
            .Where(a => a.Type == entityType && !a.IsDeleted)
            .OrderBy(a => a.AccountCode)
            .ToListAsync();
    }

    public async Task<decimal> GetBalanceAsync(Guid id)
    {
        var account = await _dbSet.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        return account?.Balance ?? 0;
    }

    public async Task<bool> CodeExistsAsync(string accountCode, Guid? excludeId = null)
    {
        var query = _dbSet.Where(a => a.AccountCode == accountCode && !a.IsDeleted);
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
