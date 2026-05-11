using Maintenance_management.application.DTOs.NewEntities;

namespace Maintenance_management.application.Interfaces;

public interface IRenovationService
{
    Task<IEnumerable<RenovationDto>> GetAllAsync();
    Task<RenovationDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<RenovationDto>> GetByUnitIdAsync(Guid unitId);
    Task<RenovationDto> CreateAsync(CreateRenovationDto dto, string createdByUserId);
    Task<RenovationDto?> UpdateAsync(Guid id, UpdateRenovationDto dto);
    Task<RenovationDto?> ApproveAsync(Guid id, string approvedByUserId);
    Task<bool> DeleteAsync(Guid id);
}