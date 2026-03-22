using Maintenance_management.application.DTOs.Technician;

namespace Maintenance_management.application.Interfaces;

public interface ITechnicianService
{
    Task<IEnumerable<TechnicianDto>> GetAllAsync();
    Task<TechnicianDto?> GetByIdAsync(Guid id);
    Task<TechnicianDto> CreateAsync(CreateTechnicianDto dto, string createdByUserId);
    Task<TechnicianDto?> UpdateAsync(Guid id, UpdateTechnicianDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> UpdateLocationAsync(Guid id, UpdateLocationDto dto);
    Task<TechnicianDto?> GetByUserIdAsync(string userId);
}
