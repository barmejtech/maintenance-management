using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IEquipmentRepository : IRepository<Equipment>
{
    Task<IEnumerable<Equipment>> GetByStatusAsync(Enums.EquipmentStatus status);
    Task<IEnumerable<Equipment>> GetDueForMaintenanceAsync();
}
