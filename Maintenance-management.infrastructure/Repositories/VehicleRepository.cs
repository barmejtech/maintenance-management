using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status)
        => await _dbSet.Where(v => v.Status == status && !v.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Vehicle>> GetDueForServiceAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(v => !v.IsDeleted && v.NextServiceDate.HasValue && v.NextServiceDate.Value <= now.AddDays(7))
            .ToListAsync();
    }
}
