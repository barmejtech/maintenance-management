using Maintenance_management.application.DTOs.Manager;

namespace Maintenance_management.application.Interfaces;

public interface IManagerService
{
    Task<IEnumerable<ManagerDto>> GetAllAsync();
    Task<ManagerDto?> GetByIdAsync(Guid id);
    Task<ManagerDto> CreateAsync(CreateManagerDto dto, string createdByUserId);
    Task<ManagerDto?> UpdateAsync(Guid id, UpdateManagerDto dto);
    Task<bool> DeleteAsync(Guid id);
}
