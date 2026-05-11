using Maintenance_management.application.DTOs.Owner;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class OwnerService : IOwnerService
{
    private readonly IOwnerRepository _repo;
    private readonly IUnitRepository _unitRepo;
    private readonly IUnitOwnershipRepository _ownershipRepo;
    private readonly IAuditLogRepository _auditLogRepo;

    public OwnerService(
        IOwnerRepository repo,
        IUnitRepository unitRepo,
        IUnitOwnershipRepository ownershipRepo,
        IAuditLogRepository auditLogRepo)
    {
        _repo = repo;
        _unitRepo = unitRepo;
        _ownershipRepo = ownershipRepo;
        _auditLogRepo = auditLogRepo;
    }

    public async Task<IEnumerable<OwnerDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<OwnerDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithHistoryAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<OwnerDto> CreateAsync(CreateOwnerDto dto)
    {
        var entity = new Owner
        {
            FullName = dto.FullName.Trim(),
            Email = dto.Email.Trim(),
            Phone = dto.Phone,
            Address = dto.Address
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<OwnerDto?> UpdateAsync(Guid id, UpdateOwnerDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.FullName = dto.FullName.Trim();
        item.Email = dto.Email.Trim();
        item.Phone = dto.Phone;
        item.Address = dto.Address;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        var withHistory = await _repo.GetWithHistoryAsync(item.Id);
        return MapToDto(withHistory ?? item);
    }

    public async Task<bool> TransferOwnershipAsync(Guid ownerId, TransferOwnershipDto dto, string performedByUserId, string? performedByName)
    {
        var owner = await _repo.GetByIdAsync(ownerId);
        if (owner is null || owner.IsDeleted) return false;

        var unit = await _unitRepo.GetByIdAsync(dto.UnitId);
        if (unit is null || unit.IsDeleted)
        {
            throw new KeyNotFoundException("Unit not found.");
        }

        var activeOwnership = await _ownershipRepo.GetActiveByUnitAsync(dto.UnitId);
        if (activeOwnership is not null)
        {
            activeOwnership.IsActive = false;
            activeOwnership.SaleDate = dto.PurchaseDate;
            activeOwnership.UpdatedAt = DateTime.UtcNow;
            await _ownershipRepo.UpdateAsync(activeOwnership);
        }

        var newOwnership = new UnitOwnership
        {
            UnitId = dto.UnitId,
            OwnerId = ownerId,
            OwnershipPercentage = dto.OwnershipPercentage,
            PurchaseDate = dto.PurchaseDate,
            IsActive = true
        };

        await _ownershipRepo.AddAsync(newOwnership);

        await _auditLogRepo.AddAsync(new AuditLog
        {
            EntityType = nameof(UnitOwnership),
            EntityId = newOwnership.Id.ToString(),
            Action = "OwnershipTransferred",
            PerformedByUserId = string.IsNullOrWhiteSpace(performedByUserId) ? "system" : performedByUserId,
            PerformedByName = string.IsNullOrWhiteSpace(performedByName) ? "System" : performedByName,
            Details = $"Unit {unit.UnitNumber} transferred to owner {owner.FullName} at {dto.OwnershipPercentage}%"
        });

        return true;
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

    private static OwnerDto MapToDto(Owner owner) => new()
    {
        Id = owner.Id,
        FullName = owner.FullName,
        Email = owner.Email,
        Phone = owner.Phone,
        Address = owner.Address,
        CreatedAt = owner.CreatedAt,
        OwnershipHistory = owner.UnitOwnerships
            .Where(h => !h.IsDeleted)
            .OrderByDescending(h => h.PurchaseDate)
            .Select(h => new OwnerUnitHistoryDto
            {
                UnitId = h.UnitId,
                UnitNumber = h.Unit?.UnitNumber ?? string.Empty,
                OwnershipPercentage = h.OwnershipPercentage,
                PurchaseDate = h.PurchaseDate,
                SaleDate = h.SaleDate,
                IsActive = h.IsActive
            })
    };
}
