using Maintenance_management.application.DTOs.UnitType;

namespace Maintenance_management.application.Interfaces;

public interface IUnitTypeService
{
    Task<IEnumerable<UnitTypeDto>> GetAllAsync();
    Task<UnitTypeDto?> GetByIdAsync(Guid id);
    Task<UnitTypeDto> CreateAsync(CreateUnitTypeDto dto);
    Task<UnitTypeDto?> UpdateAsync(Guid id, UpdateUnitTypeDto dto);
    Task<bool> DeleteAsync(Guid id);
}
