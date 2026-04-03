namespace Maintenance_management.domain.Interfaces;

public interface ITechnicianGpsLogRepository : IRepository<Entities.TechnicianGpsLog>
{
    Task<IEnumerable<Entities.TechnicianGpsLog>> GetByTechnicianIdAsync(Guid technicianId);
    Task<Entities.TechnicianGpsLog?> GetLatestByTechnicianIdAsync(Guid technicianId);
}
