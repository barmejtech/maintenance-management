using Maintenance_management.application.DTOs.MaintenanceRequest;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.Interfaces;

public interface IMaintenanceRequestService
{
    Task<IEnumerable<MaintenanceRequestDto>> GetAllAsync();
    Task<MaintenanceRequestDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<MaintenanceRequestDto>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<MaintenanceRequestDto>> GetByStatusAsync(MaintenanceRequestStatus status);
    Task<MaintenanceRequestDto> CreateAsync(CreateMaintenanceRequestDto dto);
    Task<MaintenanceRequestDto?> UpdateAsync(Guid id, UpdateMaintenanceRequestDto dto);
    Task<bool> DeleteAsync(Guid id);
}
