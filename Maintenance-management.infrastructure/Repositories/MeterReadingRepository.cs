using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class MeterReadingRepository : Repository<MeterReading>, IMeterReadingRepository
{
    public MeterReadingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<MeterReading>> GetByUnitIdAsync(Guid unitId)
        => await _dbSet
            .Include(m => m.Unit)
            .Include(m => m.Equipment)
            .Where(m => m.UnitId == unitId && !m.IsDeleted)
            .OrderByDescending(m => m.ReadingDate)
            .ToListAsync();

    public async Task<IEnumerable<MeterReading>> GetByTypeAsync(MeterType type)
        => await _dbSet
            .Include(m => m.Unit)
            .Where(m => m.Type == type && !m.IsDeleted)
            .OrderByDescending(m => m.ReadingDate)
            .ToListAsync();

    public async Task<MeterReading?> GetLatestByUnitAndTypeAsync(Guid unitId, MeterType type)
        => await _dbSet
            .Where(m => m.UnitId == unitId && m.Type == type && !m.IsDeleted)
            .OrderByDescending(m => m.ReadingDate)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<MeterReading>> GetByUnitAndTypeWithDateRangeAsync(Guid unitId, MeterType type, int months)
    {
        var cutoff = DateTime.UtcNow.AddMonths(-months);
        return await _dbSet
            .Include(m => m.Unit)
            .Where(m => m.UnitId == unitId && m.Type == type && m.ReadingDate >= cutoff && !m.IsDeleted)
            .OrderBy(m => m.ReadingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MeterReading>> GetByUnitIdAndDateRangeAsync(Guid unitId, DateTime start, DateTime end)
        => await _dbSet
            .Include(m => m.Unit)
            .Where(m => m.UnitId == unitId && m.ReadingDate >= start && m.ReadingDate <= end && !m.IsDeleted)
            .OrderBy(m => m.ReadingDate)
            .ToListAsync();
}
