using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class VendorRepository : Repository<Vendor>, IVendorRepository
{
    public VendorRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Vendor>> GetActiveAsync()
        => await _dbSet
            .Where(v => v.IsActive && !v.IsDeleted)
            .OrderBy(v => v.Name)
            .ToListAsync();

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _dbSet.Where(v => v.Email == email && !v.IsDeleted);
        if (excludeId.HasValue)
            query = query.Where(v => v.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
