using Maintenance_management.application.DTOs.PremiumService;

namespace Maintenance_management.application.Interfaces;

public interface IPremiumServiceService
{
    Task<IEnumerable<PremiumServiceDto>> GetAllAsync();
    Task<IEnumerable<PremiumServiceDto>> GetActiveAsync();
    Task<PremiumServiceDto?> GetByIdAsync(Guid id);
    Task<PremiumServiceDto> CreateAsync(CreatePremiumServiceDto dto);
    Task<PremiumServiceDto?> UpdateAsync(Guid id, UpdatePremiumServiceDto dto);
    Task<bool> DeleteAsync(Guid id);
}
