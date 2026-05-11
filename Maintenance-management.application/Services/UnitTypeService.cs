using Maintenance_management.application.DTOs.UnitType;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class UnitTypeService : IUnitTypeService
{
    private readonly IUnitTypeRepository _repo;

    public UnitTypeService(IUnitTypeRepository repo) => _repo = repo;

    public async Task<IEnumerable<UnitTypeDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<UnitTypeDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<UnitTypeDto> CreateAsync(CreateUnitTypeDto dto)
    {
        if (await _repo.NameExistsAsync(dto.Name))
        {
            throw new InvalidOperationException("Unit type name already exists.");
        }

        var entity = new UnitType
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            DefaultSizeSqm = dto.DefaultSizeSqm
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<UnitTypeDto?> UpdateAsync(Guid id, UpdateUnitTypeDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (await _repo.NameExistsAsync(dto.Name, id))
        {
            throw new InvalidOperationException("Unit type name already exists.");
        }

        item.Name = dto.Name.Trim();
        item.Description = dto.Description;
        item.DefaultSizeSqm = dto.DefaultSizeSqm;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
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

    private static UnitTypeDto MapToDto(UnitType item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        DefaultSizeSqm = item.DefaultSizeSqm,
        CreatedAt = item.CreatedAt
    };
}
