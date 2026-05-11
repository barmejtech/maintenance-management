using Maintenance_management.application.DTOs.Unit;

namespace Maintenance_management.application.Interfaces;

public interface IUnitService
{
    Task<IEnumerable<UnitDto>> GetAllAsync();
    Task<UnitDto?> GetByIdAsync(Guid id);
    Task<UnitDto> CreateAsync(CreateUnitDto dto);
    Task<UnitDto?> UpdateAsync(Guid id, UpdateUnitDto dto);
    Task<int> MassUpdateAsync(UnitMassUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
