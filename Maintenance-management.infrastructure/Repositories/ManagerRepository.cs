using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class ManagerRepository : Repository<Manager>, IManagerRepository
{
    public ManagerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Manager?> GetByUserIdAsync(string userId)
        => await _dbSet.FirstOrDefaultAsync(m => m.UserId == userId && !m.IsDeleted);
}
