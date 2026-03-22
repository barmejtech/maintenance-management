using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IAvailabilityRepository : IRepository<Availability>
{
    Task<IEnumerable<Availability>> GetByTechnicianIdAsync(Guid technicianId);
    Task<IEnumerable<Availability>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<Availability>> GetAvailableTechniciansInRangeAsync(DateTime from, DateTime to);
}
