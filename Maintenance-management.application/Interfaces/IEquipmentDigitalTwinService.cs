using Maintenance_management.application.DTOs.EquipmentDigitalTwin;

namespace Maintenance_management.application.Interfaces;

public interface IEquipmentDigitalTwinService
{
    Task<IEnumerable<EquipmentDigitalTwinDto>> GetAllAsync();
    Task<EquipmentDigitalTwinDto?> GetByEquipmentIdAsync(Guid equipmentId);
    Task<EquipmentDigitalTwinDto> UpsertAsync(UpsertDigitalTwinDto dto);
    Task<EquipmentDigitalTwinDto?> SyncFromEquipmentAsync(Guid equipmentId);
}
