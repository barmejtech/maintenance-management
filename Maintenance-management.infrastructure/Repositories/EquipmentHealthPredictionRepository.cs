using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class EquipmentHealthPredictionRepository : Repository<EquipmentHealthPrediction>, IEquipmentHealthPredictionRepository
{
    public EquipmentHealthPredictionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<EquipmentHealthPrediction?> GetByEquipmentIdAsync(Guid equipmentId)
        => await _dbSet
            .Include(p => p.Equipment)
            .FirstOrDefaultAsync(p => p.EquipmentId == equipmentId && !p.IsDeleted);

    public async Task<IEnumerable<EquipmentHealthPrediction>> GetHighRiskAsync(double threshold)
        => await _dbSet
            .Include(p => p.Equipment)
            .Where(p => !p.IsDeleted && p.FailureProbability >= threshold)
            .OrderByDescending(p => p.FailureProbability)
            .ToListAsync();
}
