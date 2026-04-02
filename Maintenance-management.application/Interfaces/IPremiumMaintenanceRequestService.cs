using Maintenance_management.application.DTOs.PremiumMaintenanceRequest;

namespace Maintenance_management.application.Interfaces;

public interface IPremiumMaintenanceRequestService
{
    Task<IEnumerable<PremiumMaintenanceRequestDto>> GetAllAsync();
    Task<IEnumerable<PremiumMaintenanceRequestDto>> GetByClientIdAsync(Guid clientId);
    Task<PremiumMaintenanceRequestDto?> GetByIdAsync(Guid id);
    Task<PremiumMaintenanceRequestDto> CreateAsync(CreatePremiumMaintenanceRequestDto dto);
    Task<PremiumMaintenanceRequestDto?> UpdateAsync(Guid id, UpdatePremiumMaintenanceRequestDto dto);
    Task<bool> DeleteAsync(Guid id);
}
