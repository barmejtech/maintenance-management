using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IHVACSystemRepository : IRepository<HVACSystem>
{
    Task<IEnumerable<HVACSystem>> GetDueForInspectionAsync();
    Task<HVACSystem?> GetByEquipmentIdAsync(Guid equipmentId);
}
