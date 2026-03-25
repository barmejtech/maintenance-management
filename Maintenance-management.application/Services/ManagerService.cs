using Maintenance_management.application.DTOs.Manager;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class ManagerService : IManagerService
{
    private readonly IManagerRepository _repo;
    private readonly IIdentityService _identityService;

    public ManagerService(IManagerRepository repo, IIdentityService identityService)
    {
        _repo = repo;
        _identityService = identityService;
    }

    public async Task<IEnumerable<ManagerDto>> GetAllAsync()
    {
        var managers = await _repo.GetAllAsync();
        return managers.Where(m => !m.IsDeleted).Select(MapToDto);
    }

    public async Task<ManagerDto?> GetByIdAsync(Guid id)
    {
        var manager = await _repo.GetByIdAsync(id);
        return manager is null || manager.IsDeleted ? null : MapToDto(manager);
    }

    public async Task<ManagerDto> CreateAsync(CreateManagerDto dto, string createdByUserId)
    {
        var (success, userId, errors) = await _identityService.CreateUserAsync(
            dto.Email, dto.FirstName, dto.LastName, dto.Password, "Manager");

        if (!success)
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", errors)}");

        var manager = new Manager
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Department = dto.Department
        };

        var created = await _repo.AddAsync(manager);
        return MapToDto(created);
    }

    public async Task<ManagerDto?> UpdateAsync(Guid id, UpdateManagerDto dto)
    {
        var manager = await _repo.GetByIdAsync(id);
        if (manager is null || manager.IsDeleted) return null;

        manager.FirstName = dto.FirstName;
        manager.LastName = dto.LastName;
        manager.Phone = dto.Phone;
        manager.Department = dto.Department;
        manager.ProfilePhotoUrl = dto.ProfilePhotoUrl ?? manager.ProfilePhotoUrl;
        manager.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(manager);
        return MapToDto(manager);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var manager = await _repo.GetByIdAsync(id);
        if (manager is null || manager.IsDeleted) return false;

        manager.IsDeleted = true;
        manager.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(manager);
        return true;
    }

    private static ManagerDto MapToDto(Manager m) => new()
    {
        Id = m.Id,
        UserId = m.UserId,
        FirstName = m.FirstName,
        LastName = m.LastName,
        Email = m.Email,
        Phone = m.Phone,
        Department = m.Department,
        ProfilePhotoUrl = m.ProfilePhotoUrl,
        CreatedAt = m.CreatedAt
    };
}
