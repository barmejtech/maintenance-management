using Maintenance_management.application.DTOs.HVAC;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class HVACService : IHVACService
{
    private readonly IHVACSystemRepository _repo;

    public HVACService(IHVACSystemRepository repo) => _repo = repo;

    public async Task<IEnumerable<HVACSystemDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(h => !h.IsDeleted).Select(MapToDto);
    }

    public async Task<HVACSystemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<HVACSystemDto> CreateAsync(CreateHVACSystemDto dto)
    {
        var entity = new HVACSystem
        {
            Name = dto.Name,
            SystemType = dto.SystemType,
            Brand = dto.Brand,
            Model = dto.Model,
            Capacity = dto.Capacity,
            CapacityUnit = dto.CapacityUnit,
            RefrigerantType = dto.RefrigerantType,
            InstallationDate = dto.InstallationDate,
            NextInspectionDate = dto.NextInspectionDate,
            Location = dto.Location,
            Notes = dto.Notes,
            EquipmentId = dto.EquipmentId
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<HVACSystemDto?> UpdateAsync(Guid id, UpdateHVACSystemDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Name = dto.Name;
        item.SystemType = dto.SystemType;
        item.Brand = dto.Brand;
        item.Model = dto.Model;
        item.Capacity = dto.Capacity;
        item.CapacityUnit = dto.CapacityUnit;
        item.RefrigerantType = dto.RefrigerantType;
        item.InstallationDate = dto.InstallationDate;
        item.LastInspectionDate = dto.LastInspectionDate;
        item.NextInspectionDate = dto.NextInspectionDate;
        item.Location = dto.Location;
        item.Notes = dto.Notes;
        item.EquipmentId = dto.EquipmentId;
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

    public async Task<IEnumerable<HVACSystemDto>> GetDueForInspectionAsync()
    {
        var items = await _repo.GetDueForInspectionAsync();
        return items.Select(MapToDto);
    }

    private static HVACSystemDto MapToDto(HVACSystem h) => new()
    {
        Id = h.Id,
        Name = h.Name,
        SystemType = h.SystemType,
        Brand = h.Brand,
        Model = h.Model,
        Capacity = h.Capacity,
        CapacityUnit = h.CapacityUnit,
        RefrigerantType = h.RefrigerantType,
        InstallationDate = h.InstallationDate,
        LastInspectionDate = h.LastInspectionDate,
        NextInspectionDate = h.NextInspectionDate,
        Location = h.Location,
        Notes = h.Notes,
        EquipmentId = h.EquipmentId,
        CreatedAt = h.CreatedAt
    };
}
