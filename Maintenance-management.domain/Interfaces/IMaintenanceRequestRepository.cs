using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Interfaces;

public interface IMaintenanceRequestRepository : IRepository<MaintenanceRequest>
{
    Task<IEnumerable<MaintenanceRequest>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<MaintenanceRequest>> GetByStatusAsync(MaintenanceRequestStatus status);
    Task<MaintenanceRequest?> GetWithDetailsAsync(Guid id);
    Task AddAssignmentAsync(MaintenanceRequestAssignment assignment);
    Task RemoveAssignmentsAsync(Guid requestId);
}
