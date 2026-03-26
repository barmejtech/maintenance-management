using Maintenance_management.application.DTOs.DataEntry;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class DataEntryService : IDataEntryService
{
    private readonly IDataEntryRepository _repo;
    private readonly IIdentityService _identityService;

    public DataEntryService(IDataEntryRepository repo, IIdentityService identityService)
    {
        _repo = repo;
        _identityService = identityService;
    }

    public async Task<IEnumerable<DataEntryDto>> GetAllAsync()
    {
        var entries = await _repo.GetAllAsync();
        return entries.Where(e => !e.IsDeleted).Select(MapToDto);
    }

    public async Task<DataEntryDto?> GetByIdAsync(Guid id)
    {
        var entry = await _repo.GetByIdAsync(id);
        return entry is null || entry.IsDeleted ? null : MapToDto(entry);
    }

    public async Task<DataEntryDto> CreateAsync(CreateDataEntryDto dto, string createdByUserId)
    {
        var (success, userId, errors) = await _identityService.CreateUserAsync(
            dto.Email, dto.FirstName, dto.LastName, dto.Password, "DataEntry");

        if (!success)
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", errors)}");

        var entity = new Maintenance_management.domain.Entities.DataEntry
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Section = dto.Section
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<DataEntryDto?> UpdateAsync(Guid id, UpdateDataEntryDto dto)
    {
        var entry = await _repo.GetByIdAsync(id);
        if (entry is null || entry.IsDeleted) return null;

        entry.FirstName = dto.FirstName;
        entry.LastName = dto.LastName;
        entry.Phone = dto.Phone;
        entry.Section = dto.Section;
        entry.ProfilePhotoUrl = dto.ProfilePhotoUrl ?? entry.ProfilePhotoUrl;
        entry.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(entry);
        return MapToDto(entry);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entry = await _repo.GetByIdAsync(id);
        if (entry is null || entry.IsDeleted) return false;

        entry.IsDeleted = true;
        entry.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(entry);
        return true;
    }

    private static DataEntryDto MapToDto(Maintenance_management.domain.Entities.DataEntry e) => new()
    {
        Id = e.Id,
        UserId = e.UserId,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        Phone = e.Phone,
        Section = e.Section,
        ProfilePhotoUrl = e.ProfilePhotoUrl,
        CreatedAt = e.CreatedAt
    };
}
