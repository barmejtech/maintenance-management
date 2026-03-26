using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class DataEntryRepository : Repository<DataEntry>, IDataEntryRepository
{
    public DataEntryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<DataEntry?> GetByUserIdAsync(string userId)
        => await _dbSet.FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted);
}
