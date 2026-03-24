using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class SparePartRepository : Repository<SparePart>, ISparePartRepository
{
    public SparePartRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<SparePart>> GetLowStockAsync()
        => await _dbSet
            .Where(p => !p.IsDeleted && p.QuantityInStock <= p.MinimumStockLevel)
            .ToListAsync();

    public async Task<IEnumerable<SparePartUsage>> GetUsagesBySparePartIdAsync(Guid sparePartId)
        => await _context.SparePartUsages
            .Include(u => u.SparePart)
            .Include(u => u.TaskOrder)
            .Where(u => u.SparePartId == sparePartId && !u.IsDeleted)
            .OrderByDescending(u => u.UsedAt)
            .ToListAsync();

    public async Task<IEnumerable<SparePartUsage>> GetUsagesByTaskOrderIdAsync(Guid taskOrderId)
        => await _context.SparePartUsages
            .Include(u => u.SparePart)
            .Include(u => u.TaskOrder)
            .Where(u => u.TaskOrderId == taskOrderId && !u.IsDeleted)
            .OrderByDescending(u => u.UsedAt)
            .ToListAsync();

    public async Task<SparePartUsage?> GetUsageByIdAsync(Guid usageId)
        => await _context.SparePartUsages
            .Include(u => u.SparePart)
            .Include(u => u.TaskOrder)
            .FirstOrDefaultAsync(u => u.Id == usageId && !u.IsDeleted);

    public async Task<SparePartUsage> AddUsageAsync(SparePartUsage usage)
    {
        await _context.SparePartUsages.AddAsync(usage);
        await _context.SaveChangesAsync();
        return usage;
    }

    public async Task UpdateUsageAsync(SparePartUsage usage)
    {
        _context.SparePartUsages.Update(usage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUsageAsync(Guid usageId)
    {
        var usage = await _context.SparePartUsages.FindAsync(usageId);
        if (usage is not null)
        {
            usage.IsDeleted = true;
            usage.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
