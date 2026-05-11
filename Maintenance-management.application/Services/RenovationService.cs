using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class RenovationService : IRenovationService
{
    private readonly IRenovationRepository _repo;
    private readonly IUnitRepository _unitRepo;

    public RenovationService(IRenovationRepository repo, IUnitRepository unitRepo)
    {
        _repo = repo;
        _unitRepo = unitRepo;
    }

    public async Task<IEnumerable<RenovationDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<RenovationDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<RenovationDto>> GetByUnitIdAsync(Guid unitId)
    {
        var items = await _repo.GetByUnitIdAsync(unitId);
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<RenovationDto> CreateAsync(CreateRenovationDto dto, string createdByUserId)
    {
        if (!await _unitRepo.ExistsAsync(dto.UnitId))
            throw new KeyNotFoundException("Unit not found.");

        var entity = new Renovation
        {
            UnitId = dto.UnitId,
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Budget = dto.Budget,
            ContractorName = dto.ContractorName,
            ContractorPhone = dto.ContractorPhone,
            Notes = dto.Notes,
            Status = RenovationStatus.Planned
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<RenovationDto?> UpdateAsync(Guid id, UpdateRenovationDto dto)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Title = dto.Title;
        item.Description = dto.Description;
        item.StartDate = dto.StartDate;
        item.EndDate = dto.EndDate;
        item.Status = dto.Status;
        item.Budget = dto.Budget;
        item.ActualCost = dto.ActualCost;
        item.ContractorName = dto.ContractorName;
        item.ContractorPhone = dto.ContractorPhone;
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

    public async Task<RenovationDto?> ApproveAsync(Guid id, string approvedByUserId)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Status = RenovationStatus.InProgress;
        item.ApprovedByUserId = approvedByUserId;
        item.ApprovedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    private static RenovationDto MapToDto(Renovation r) => new()
    {
        Id = r.Id,
        UnitId = r.UnitId,
        UnitNumber = r.Unit?.UnitNumber ?? string.Empty,
        Title = r.Title,
        Description = r.Description,
        StartDate = r.StartDate,
        EndDate = r.EndDate,
        Status = r.Status,
        Budget = r.Budget,
        ActualCost = r.ActualCost,
        ContractorName = r.ContractorName,
        ContractorPhone = r.ContractorPhone,
        ApprovedByUserId = r.ApprovedByUserId,
        ApprovedAt = r.ApprovedAt,
        Notes = r.Notes,
        CreatedAt = r.CreatedAt
    };
}