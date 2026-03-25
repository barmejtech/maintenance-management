using Maintenance_management.application.DTOs.Equipment;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _repo;

    public EquipmentService(IEquipmentRepository repo) => _repo = repo;

    public async Task<IEnumerable<EquipmentDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(e => !e.IsDeleted).Select(MapToDto);
    }

    public async Task<EquipmentDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto)
    {
        var entity = new Equipment
        {
            Name = dto.Name,
            SerialNumber = dto.SerialNumber,
            Model = dto.Model,
            Manufacturer = dto.Manufacturer,
            Location = dto.Location,
            InstallationDate = dto.InstallationDate,
            NextMaintenanceDate = dto.NextMaintenanceDate,
            Notes = dto.Notes,
            BeforeMaintenancePhotoUrl = dto.BeforeMaintenancePhotoUrl,
            AfterMaintenancePhotoUrl = dto.AfterMaintenancePhotoUrl,
            Photo3Url = dto.Photo3Url,
            Photo4Url = dto.Photo4Url
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<EquipmentDto?> UpdateAsync(Guid id, UpdateEquipmentDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Name = dto.Name;
        item.SerialNumber = dto.SerialNumber;
        item.Model = dto.Model;
        item.Manufacturer = dto.Manufacturer;
        item.Location = dto.Location;
        item.InstallationDate = dto.InstallationDate;
        item.LastMaintenanceDate = dto.LastMaintenanceDate;
        item.NextMaintenanceDate = dto.NextMaintenanceDate;
        item.Status = dto.Status;
        item.Notes = dto.Notes;
        item.BeforeMaintenancePhotoUrl = dto.BeforeMaintenancePhotoUrl;
        item.AfterMaintenancePhotoUrl = dto.AfterMaintenancePhotoUrl;
        item.Photo3Url = dto.Photo3Url;
        item.Photo4Url = dto.Photo4Url;
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

    public async Task<IEnumerable<EquipmentDto>> GetDueForMaintenanceAsync()
    {
        var items = await _repo.GetDueForMaintenanceAsync();
        return items.Select(MapToDto);
    }

    private static EquipmentDto MapToDto(Equipment e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        SerialNumber = e.SerialNumber,
        Model = e.Model,
        Manufacturer = e.Manufacturer,
        Location = e.Location,
        InstallationDate = e.InstallationDate,
        LastMaintenanceDate = e.LastMaintenanceDate,
        NextMaintenanceDate = e.NextMaintenanceDate,
        Status = e.Status,
        Notes = e.Notes,
        BeforeMaintenancePhotoUrl = e.BeforeMaintenancePhotoUrl,
        AfterMaintenancePhotoUrl = e.AfterMaintenancePhotoUrl,
        Photo3Url = e.Photo3Url,
        Photo4Url = e.Photo4Url,
        CreatedAt = e.CreatedAt
    };
}
