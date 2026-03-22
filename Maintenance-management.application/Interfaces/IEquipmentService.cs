using Maintenance_management.application.DTOs.Equipment;

namespace Maintenance_management.application.Interfaces;

public interface IEquipmentService
{
    Task<IEnumerable<EquipmentDto>> GetAllAsync();
    Task<EquipmentDto?> GetByIdAsync(Guid id);
    Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto);
    Task<EquipmentDto?> UpdateAsync(Guid id, UpdateEquipmentDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<EquipmentDto>> GetDueForMaintenanceAsync();
}
