using Maintenance_management.application.DTOs.Technician;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class TechnicianService : ITechnicianService
{
    private readonly ITechnicianRepository _repo;
    private readonly IIdentityService _identityService;

    public TechnicianService(ITechnicianRepository repo, IIdentityService identityService)
    {
        _repo = repo;
        _identityService = identityService;
    }

    public async Task<IEnumerable<TechnicianDto>> GetAllAsync()
    {
        var technicians = await _repo.GetAllAsync();
        return technicians.Where(t => !t.IsDeleted).Select(MapToDto);
    }

    public async Task<TechnicianDto?> GetByIdAsync(Guid id)
    {
        var tech = await _repo.GetByIdAsync(id);
        return tech is null || tech.IsDeleted ? null : MapToDto(tech);
    }

    public async Task<TechnicianDto?> GetByUserIdAsync(string userId)
    {
        var tech = await _repo.GetByUserIdAsync(userId);
        return tech is null ? null : MapToDto(tech);
    }

    public async Task<TechnicianDto> CreateAsync(CreateTechnicianDto dto, string createdByUserId)
    {
        var (success, userId, errors) = await _identityService.CreateUserAsync(
            dto.Email, dto.FirstName, dto.LastName, dto.Password, "Technician");

        if (!success)
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", errors)}");

        var technician = new Technician
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Specialization = dto.Specialization,
            Status = TechnicianStatus.Available
        };

        var created = await _repo.AddAsync(technician);
        return MapToDto(created);
    }

    public async Task<TechnicianDto?> UpdateAsync(Guid id, UpdateTechnicianDto dto)
    {
        var tech = await _repo.GetByIdAsync(id);
        if (tech is null || tech.IsDeleted) return null;

        tech.FirstName = dto.FirstName;
        tech.LastName = dto.LastName;
        tech.Phone = dto.Phone;
        tech.Specialization = dto.Specialization;
        tech.Status = dto.Status;
        tech.ProfilePhotoUrl = dto.ProfilePhotoUrl ?? tech.ProfilePhotoUrl;
        tech.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(tech);
        return MapToDto(tech);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tech = await _repo.GetByIdAsync(id);
        if (tech is null || tech.IsDeleted) return false;

        tech.IsDeleted = true;
        tech.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(tech);
        return true;
    }

    public async Task<bool> UpdateLocationAsync(Guid id, UpdateLocationDto dto)
    {
        var tech = await _repo.GetByIdAsync(id);
        if (tech is null || tech.IsDeleted) return false;

        await _repo.UpdateLocationAsync(id, dto.Latitude, dto.Longitude);
        return true;
    }

    public async Task<IEnumerable<TechnicianDto>> GetAvailableAsync()
    {
        var technicians = await _repo.GetAvailableTechniciansAsync();
        return technicians.Where(t => !t.IsDeleted).Select(MapToDto);
    }

    private static TechnicianDto MapToDto(Technician t) => new()
    {
        Id = t.Id,
        UserId = t.UserId,
        FirstName = t.FirstName,
        LastName = t.LastName,
        Email = t.Email,
        Phone = t.Phone,
        Specialization = t.Specialization,
        ProfilePhotoUrl = t.ProfilePhotoUrl,
        Status = t.Status,
        Latitude = t.Latitude,
        Longitude = t.Longitude,
        LastLocationUpdate = t.LastLocationUpdate,
        CreatedAt = t.CreatedAt
    };
}
