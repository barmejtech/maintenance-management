using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IEquipmentDigitalTwinRepository : IRepository<EquipmentDigitalTwin>
{
    Task<EquipmentDigitalTwin?> GetByEquipmentIdAsync(Guid equipmentId);
    Task<IEnumerable<EquipmentDigitalTwin>> GetAllWithEquipmentAsync();
}
