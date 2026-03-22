using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class TechnicianRepository : Repository<Technician>, ITechnicianRepository
{
    public TechnicianRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Technician?> GetByUserIdAsync(string userId)
        => await _dbSet.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);

    public async Task<IEnumerable<Technician>> GetByGroupIdAsync(Guid groupId)
        => await _context.TechnicianGroupMembers
            .Where(m => m.GroupId == groupId && !m.IsDeleted)
            .Select(m => m.Technician)
            .Where(t => !t.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Technician>> GetAvailableTechniciansAsync()
        => await _dbSet
            .Where(t => !t.IsDeleted && t.Status == domain.Enums.TechnicianStatus.Available)
            .ToListAsync();

    public async Task UpdateLocationAsync(Guid technicianId, double latitude, double longitude)
    {
        var tech = await _dbSet.FindAsync(technicianId);
        if (tech is not null)
        {
            tech.Latitude = latitude;
            tech.Longitude = longitude;
            tech.LastLocationUpdate = DateTime.UtcNow;
            tech.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
