using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface ITechnicianRepository : IRepository<Technician>
{
    Task<Technician?> GetByUserIdAsync(string userId);
    Task<IEnumerable<Technician>> GetByGroupIdAsync(Guid groupId);
    Task<IEnumerable<Technician>> GetAvailableTechniciansAsync();
    Task UpdateLocationAsync(Guid technicianId, double latitude, double longitude);
}
