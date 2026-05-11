using Maintenance_management.application.DTOs.NewEntities;

namespace Maintenance_management.application.Interfaces;

public interface IVendorService
{
    Task<IEnumerable<VendorDto>> GetAllAsync();
    Task<VendorDto?> GetByIdAsync(Guid id);
    Task<VendorDto> CreateAsync(CreateVendorDto dto);
    Task<VendorDto?> UpdateAsync(Guid id, UpdateVendorDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<VendorDto>> GetActiveAsync();
}