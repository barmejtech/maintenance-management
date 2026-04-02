using Maintenance_management.application.DTOs.PremiumService;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class PremiumServiceService : IPremiumServiceService
{
    private readonly IPremiumServiceRepository _repo;

    public PremiumServiceService(IPremiumServiceRepository repo) => _repo = repo;

    public async Task<IEnumerable<PremiumServiceDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(s => !s.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<PremiumServiceDto>> GetActiveAsync()
    {
        var items = await _repo.GetActiveAsync();
        return items.Select(MapToDto);
    }

    public async Task<PremiumServiceDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<PremiumServiceDto> CreateAsync(CreatePremiumServiceDto dto)
    {
        var entity = new PremiumService
        {
            Name = dto.Name,
            Description = dto.Description,
            ServiceType = dto.ServiceType,
            Price = dto.Price,
            DurationHours = dto.DurationHours,
            PriorityLevel = dto.PriorityLevel,
            IsActive = dto.IsActive,
            Features = dto.Features
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<PremiumServiceDto?> UpdateAsync(Guid id, UpdatePremiumServiceDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.ServiceType = dto.ServiceType;
        item.Price = dto.Price;
        item.DurationHours = dto.DurationHours;
        item.PriorityLevel = dto.PriorityLevel;
        item.IsActive = dto.IsActive;
        item.Features = dto.Features;
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

    private static PremiumServiceDto MapToDto(PremiumService s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
        ServiceType = s.ServiceType,
        Price = s.Price,
        DurationHours = s.DurationHours,
        PriorityLevel = s.PriorityLevel,
        IsActive = s.IsActive,
        Features = s.Features,
        CreatedAt = s.CreatedAt
    };
}
