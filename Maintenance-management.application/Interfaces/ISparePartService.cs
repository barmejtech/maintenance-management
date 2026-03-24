using Maintenance_management.application.DTOs.SparePart;

namespace Maintenance_management.application.Interfaces;

public interface ISparePartService
{
    Task<IEnumerable<SparePartDto>> GetAllAsync();
    Task<SparePartDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<SparePartDto>> GetLowStockAsync();
    Task<SparePartDto> CreateAsync(CreateSparePartDto dto);
    Task<SparePartDto?> UpdateAsync(Guid id, UpdateSparePartDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<SparePartUsageDto>> GetUsagesBySparePartIdAsync(Guid sparePartId);
    Task<IEnumerable<SparePartUsageDto>> GetUsagesByTaskOrderIdAsync(Guid taskOrderId);
    Task<SparePartUsageDto> AddUsageAsync(CreateSparePartUsageDto dto, string usedByUserId);
    Task<bool> DeleteUsageAsync(Guid usageId);
}
