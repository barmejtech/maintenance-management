using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface ITechnicianPerformanceScoreRepository : IRepository<TechnicianPerformanceScore>
{
    Task<TechnicianPerformanceScore?> GetByTechnicianIdAsync(Guid technicianId);
    Task<IEnumerable<TechnicianPerformanceScore>> GetTopPerformersAsync(int count);
}
