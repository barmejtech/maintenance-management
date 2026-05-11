using Maintenance_management.application.DTOs.Tenant;

namespace Maintenance_management.application.Interfaces;

public interface ITenantService
{
    Task<IEnumerable<TenantDto>> GetAllAsync();
    Task<TenantDto?> GetByIdAsync(Guid id);
    Task<TenantDto> CreateAsync(CreateTenantDto dto);
    Task<TenantDto?> UpdateAsync(Guid id, UpdateTenantDto dto);
    Task<bool> DeleteAsync(Guid id);
}
