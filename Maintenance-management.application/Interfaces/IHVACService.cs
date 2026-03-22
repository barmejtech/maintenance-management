using Maintenance_management.application.DTOs.HVAC;

namespace Maintenance_management.application.Interfaces;

public interface IHVACService
{
    Task<IEnumerable<HVACSystemDto>> GetAllAsync();
    Task<HVACSystemDto?> GetByIdAsync(Guid id);
    Task<HVACSystemDto> CreateAsync(CreateHVACSystemDto dto);
    Task<HVACSystemDto?> UpdateAsync(Guid id, UpdateHVACSystemDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<HVACSystemDto>> GetDueForInspectionAsync();
}
