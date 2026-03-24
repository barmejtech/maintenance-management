using Maintenance_management.application.DTOs.SparePart;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class SparePartService : ISparePartService
{
    private readonly ISparePartRepository _repo;

    public SparePartService(ISparePartRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<SparePartDto>> GetAllAsync()
    {
        var parts = await _repo.GetAllAsync();
        return parts.Select(MapToDto);
    }

    public async Task<SparePartDto?> GetByIdAsync(Guid id)
    {
        var part = await _repo.GetByIdAsync(id);
        return part is null ? null : MapToDto(part);
    }

    public async Task<IEnumerable<SparePartDto>> GetLowStockAsync()
    {
        var parts = await _repo.GetLowStockAsync();
        return parts.Select(MapToDto);
    }

    public async Task<SparePartDto> CreateAsync(CreateSparePartDto dto)
    {
        var part = new SparePart
        {
            Name = dto.Name,
            PartNumber = dto.PartNumber,
            Description = dto.Description,
            Unit = dto.Unit,
            QuantityInStock = dto.QuantityInStock,
            MinimumStockLevel = dto.MinimumStockLevel,
            UnitPrice = dto.UnitPrice,
            Supplier = dto.Supplier,
            StorageLocation = dto.StorageLocation,
            Notes = dto.Notes,
            Photo1Url = dto.Photo1Url,
            Photo2Url = dto.Photo2Url,
            Photo3Url = dto.Photo3Url,
            Photo4Url = dto.Photo4Url
        };

        var created = await _repo.AddAsync(part);
        return MapToDto(created);
    }

    public async Task<SparePartDto?> UpdateAsync(Guid id, UpdateSparePartDto dto)
    {
        var part = await _repo.GetByIdAsync(id);
        if (part is null) return null;

        part.Name = dto.Name;
        part.PartNumber = dto.PartNumber;
        part.Description = dto.Description;
        part.Unit = dto.Unit;
        part.QuantityInStock = dto.QuantityInStock;
        part.MinimumStockLevel = dto.MinimumStockLevel;
        part.UnitPrice = dto.UnitPrice;
        part.Supplier = dto.Supplier;
        part.StorageLocation = dto.StorageLocation;
        part.Notes = dto.Notes;
        part.Photo1Url = dto.Photo1Url;
        part.Photo2Url = dto.Photo2Url;
        part.Photo3Url = dto.Photo3Url;
        part.Photo4Url = dto.Photo4Url;
        part.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(part);
        return MapToDto(part);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _repo.ExistsAsync(id)) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<SparePartUsageDto>> GetUsagesBySparePartIdAsync(Guid sparePartId)
    {
        var usages = await _repo.GetUsagesBySparePartIdAsync(sparePartId);
        return usages.Select(MapUsageToDto);
    }

    public async Task<IEnumerable<SparePartUsageDto>> GetUsagesByTaskOrderIdAsync(Guid taskOrderId)
    {
        var usages = await _repo.GetUsagesByTaskOrderIdAsync(taskOrderId);
        return usages.Select(MapUsageToDto);
    }

    public async Task<SparePartUsageDto> AddUsageAsync(CreateSparePartUsageDto dto, string usedByUserId)
    {
        var part = await _repo.GetByIdAsync(dto.SparePartId)
            ?? throw new InvalidOperationException("Spare part not found.");

        if (part.QuantityInStock < dto.QuantityUsed)
            throw new InvalidOperationException("Insufficient stock.");

        var usage = new SparePartUsage
        {
            SparePartId = dto.SparePartId,
            TaskOrderId = dto.TaskOrderId,
            QuantityUsed = dto.QuantityUsed,
            Notes = dto.Notes,
            UsedAt = DateTime.UtcNow,
            UsedByUserId = usedByUserId
        };

        part.QuantityInStock -= dto.QuantityUsed;
        part.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(part);

        var created = await _repo.AddUsageAsync(usage);
        return MapUsageToDto(created);
    }

    public async Task<bool> DeleteUsageAsync(Guid usageId)
    {
        var usage = await _repo.GetUsageByIdAsync(usageId);
        if (usage is null) return false;

        // Restore stock
        var part = await _repo.GetByIdAsync(usage.SparePartId);
        if (part is not null)
        {
            part.QuantityInStock += usage.QuantityUsed;
            part.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(part);
        }

        await _repo.DeleteUsageAsync(usageId);
        return true;
    }

    private static SparePartDto MapToDto(SparePart p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        PartNumber = p.PartNumber,
        Description = p.Description,
        Unit = p.Unit,
        QuantityInStock = p.QuantityInStock,
        MinimumStockLevel = p.MinimumStockLevel,
        UnitPrice = p.UnitPrice,
        Supplier = p.Supplier,
        StorageLocation = p.StorageLocation,
        Notes = p.Notes,
        Photo1Url = p.Photo1Url,
        Photo2Url = p.Photo2Url,
        Photo3Url = p.Photo3Url,
        Photo4Url = p.Photo4Url,
        CreatedAt = p.CreatedAt
    };

    private static SparePartUsageDto MapUsageToDto(SparePartUsage u) => new()
    {
        Id = u.Id,
        SparePartId = u.SparePartId,
        SparePartName = u.SparePart?.Name ?? string.Empty,
        TaskOrderId = u.TaskOrderId,
        TaskOrderTitle = u.TaskOrder?.Title,
        QuantityUsed = u.QuantityUsed,
        Notes = u.Notes,
        UsedAt = u.UsedAt,
        UsedByUserId = u.UsedByUserId,
        CreatedAt = u.CreatedAt
    };
}
