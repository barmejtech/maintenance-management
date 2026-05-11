using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Interfaces;

public interface IRenovationRepository : IRepository<Renovation>
{
    Task<Renovation?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Renovation>> GetByUnitIdAsync(Guid unitId);
    Task<IEnumerable<Renovation>> GetByStatusAsync(Entities.RenovationStatus status);
    Task<IEnumerable<Renovation>> GetInProgressAsync();
}