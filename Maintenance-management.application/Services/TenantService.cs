using Maintenance_management.application.DTOs.Tenant;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _repo;
    private readonly IUnitRepository _unitRepo;

    public TenantService(ITenantRepository repo, IUnitRepository unitRepo)
    {
        _repo = repo;
        _unitRepo = unitRepo;
    }

    public async Task<IEnumerable<TenantDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        var unit = await _unitRepo.GetByIdAsync(item.UnitId);
        item.Unit = unit;
        return MapToDto(item);
    }

    public async Task<TenantDto> CreateAsync(CreateTenantDto dto)
    {
        if (!await _unitRepo.ExistsAsync(dto.UnitId))
        {
            throw new KeyNotFoundException("Unit not found.");
        }

        var entity = new Tenant
        {
            UnitId = dto.UnitId,
            FullName = dto.FullName.Trim(),
            Email = dto.Email.Trim(),
            Phone = dto.Phone,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            LeaseStartDate = dto.LeaseStartDate,
            LeaseEndDate = dto.LeaseEndDate,
            RentalAmount = dto.RentalAmount,
            DepositAmount = dto.DepositAmount
        };

        var created = await _repo.AddAsync(entity);
        var unit = await _unitRepo.GetByIdAsync(created.UnitId);
        created.Unit = unit;
        return MapToDto(created);
    }

    public async Task<TenantDto?> UpdateAsync(Guid id, UpdateTenantDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (!await _unitRepo.ExistsAsync(dto.UnitId))
        {
            throw new KeyNotFoundException("Unit not found.");
        }

        item.UnitId = dto.UnitId;
        item.FullName = dto.FullName.Trim();
        item.Email = dto.Email.Trim();
        item.Phone = dto.Phone;
        item.EmergencyContactName = dto.EmergencyContactName;
        item.EmergencyContactPhone = dto.EmergencyContactPhone;
        item.LeaseStartDate = dto.LeaseStartDate;
        item.LeaseEndDate = dto.LeaseEndDate;
        item.RentalAmount = dto.RentalAmount;
        item.DepositAmount = dto.DepositAmount;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        var unit = await _unitRepo.GetByIdAsync(item.UnitId);
        item.Unit = unit;
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

    private static TenantDto MapToDto(Tenant tenant) => new()
    {
        Id = tenant.Id,
        UnitId = tenant.UnitId,
        UnitNumber = tenant.Unit?.UnitNumber ?? string.Empty,
        FullName = tenant.FullName,
        Email = tenant.Email,
        Phone = tenant.Phone,
        EmergencyContactName = tenant.EmergencyContactName,
        EmergencyContactPhone = tenant.EmergencyContactPhone,
        LeaseStartDate = tenant.LeaseStartDate,
        LeaseEndDate = tenant.LeaseEndDate,
        RentalAmount = tenant.RentalAmount,
        DepositAmount = tenant.DepositAmount,
        CreatedAt = tenant.CreatedAt
    };
}
