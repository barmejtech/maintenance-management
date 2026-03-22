using Maintenance_management.application.DTOs.Availability;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IAvailabilityRepository _repo;

    public AvailabilityService(IAvailabilityRepository repo) => _repo = repo;

    public async Task<IEnumerable<AvailabilityDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(a => !a.IsDeleted).Select(MapToDto);
    }

    public async Task<AvailabilityDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<AvailabilityDto>> GetByTechnicianIdAsync(Guid technicianId)
    {
        var items = await _repo.GetByTechnicianIdAsync(technicianId);
        return items.Where(a => !a.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<AvailabilityDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var items = await _repo.GetByDateRangeAsync(from, to);
        return items.Where(a => !a.IsDeleted).Select(MapToDto);
    }

    public async Task<AvailabilityDto> CreateAsync(CreateAvailabilityDto dto)
    {
        var entity = new Availability
        {
            TechnicianId = dto.TechnicianId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsAvailable = dto.IsAvailable,
            Notes = dto.Notes
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<AvailabilityDto?> UpdateAsync(Guid id, UpdateAvailabilityDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.TechnicianId = dto.TechnicianId;
        item.StartTime = dto.StartTime;
        item.EndTime = dto.EndTime;
        item.IsAvailable = dto.IsAvailable;
        item.Notes = dto.Notes;
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

    private static AvailabilityDto MapToDto(Availability a) => new()
    {
        Id = a.Id,
        TechnicianId = a.TechnicianId,
        TechnicianName = a.Technician is not null
            ? $"{a.Technician.FirstName} {a.Technician.LastName}"
            : string.Empty,
        StartTime = a.StartTime,
        EndTime = a.EndTime,
        IsAvailable = a.IsAvailable,
        Notes = a.Notes,
        CreatedAt = a.CreatedAt
    };
}
