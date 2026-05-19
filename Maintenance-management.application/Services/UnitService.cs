using Maintenance_management.application.DTOs.Unit;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _repo;
    private readonly IUnitTypeRepository _unitTypeRepo;

    public UnitService(IUnitRepository repo, IUnitTypeRepository unitTypeRepo)
    {
        _repo = repo;
        _unitTypeRepo = unitTypeRepo;
    }

    public async Task<IEnumerable<UnitDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<UnitDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<UnitDto> CreateAsync(CreateUnitDto dto)
    {
        if (!await _unitTypeRepo.ExistsAsync(dto.UnitTypeId))
        {
            throw new KeyNotFoundException("Unit type not found.");
        }

        if (await _repo.UnitNumberExistsAsync(dto.UnitNumber))
        {
            throw new InvalidOperationException("Unit number already exists.");
        }

        var entity = new Unit
        {
            UnitNumber = dto.UnitNumber.Trim(),
            Floor = dto.Floor,
            SizeSqm = dto.SizeSqm,
            Status = dto.Status,
            ShareValue = dto.ShareValue,
            UnitTypeId = dto.UnitTypeId
        };

        var created = await _repo.AddAsync(entity);
        var withDetails = await _repo.GetWithDetailsAsync(created.Id);
        return MapToDto(withDetails ?? created);
    }

    public async Task<UnitDto?> UpdateAsync(Guid id, UpdateUnitDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (!await _unitTypeRepo.ExistsAsync(dto.UnitTypeId))
        {
            throw new KeyNotFoundException("Unit type not found.");
        }

        if (await _repo.UnitNumberExistsAsync(dto.UnitNumber, id))
        {
            throw new InvalidOperationException("Unit number already exists.");
        }

        item.UnitNumber = dto.UnitNumber.Trim();
        item.Floor = dto.Floor;
        item.SizeSqm = dto.SizeSqm;
        item.Status = dto.Status;
        item.ShareValue = dto.ShareValue;
        item.UnitTypeId = dto.UnitTypeId;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        var withDetails = await _repo.GetWithDetailsAsync(item.Id);
        return MapToDto(withDetails ?? item);
    }

    public async Task<int> MassUpdateAsync(UnitMassUpdateDto dto)
    {
        var ids = dto.UnitIds?.Distinct().ToList() ?? [];
        if (ids.Count == 0) return 0;

        var units = (await _repo.GetByIdsAsync(ids)).ToList();
        foreach (var unit in units)
        {
            if (dto.Status.HasValue)
            {
                unit.Status = dto.Status.Value;
            }

            if (dto.Floor.HasValue)
            {
                unit.Floor = dto.Floor;
            }

            unit.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(unit);
        }

        return units.Count;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return false;

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(item);
        return true;
    }

    private static UnitDto MapToDto(Unit item) => new()
    {
        Id = item.Id,
        UnitNumber = item.UnitNumber,
        Floor = item.Floor,
        SizeSqm = item.SizeSqm,
        Status = item.Status,
        ShareValue = item.ShareValue,
        UnitTypeId = item.UnitTypeId,
        UnitTypeName = item.UnitType?.Name ?? string.Empty,
        CreatedAt = item.CreatedAt,
        OwnershipHistory = item.OwnershipHistory
            .Where(h => !h.IsDeleted)
            .OrderByDescending(h => h.PurchaseDate)
            .Select(h => new UnitOwnershipHistoryDto
            {
                Id = h.Id,
                OwnerId = h.OwnerId,
                OwnerName = h.Owner?.FullName ?? string.Empty,
                OwnershipPercentage = h.OwnershipPercentage,
                PurchaseDate = h.PurchaseDate,
                SaleDate = h.SaleDate,
                IsActive = h.IsActive
            }).ToList()
    };
}
