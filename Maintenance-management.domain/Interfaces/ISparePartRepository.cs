using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface ISparePartRepository : IRepository<SparePart>
{
    Task<IEnumerable<SparePart>> GetLowStockAsync();
    Task<IEnumerable<SparePartUsage>> GetUsagesBySparePartIdAsync(Guid sparePartId);
    Task<IEnumerable<SparePartUsage>> GetUsagesByTaskOrderIdAsync(Guid taskOrderId);
    Task<SparePartUsage?> GetUsageByIdAsync(Guid usageId);
    Task<SparePartUsage> AddUsageAsync(SparePartUsage usage);
    Task UpdateUsageAsync(SparePartUsage usage);
    Task DeleteUsageAsync(Guid usageId);
}
