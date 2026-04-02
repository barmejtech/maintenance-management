using Maintenance_management.application.DTOs.PremiumMaintenanceRequest;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class PremiumMaintenanceRequestService : IPremiumMaintenanceRequestService
{
    private readonly IPremiumMaintenanceRequestRepository _repo;

    public PremiumMaintenanceRequestService(IPremiumMaintenanceRequestRepository repo) => _repo = repo;

    public async Task<IEnumerable<PremiumMaintenanceRequestDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        var result = new List<PremiumMaintenanceRequestDto>();
        foreach (var item in items.Where(r => !r.IsDeleted))
        {
            var detailed = await _repo.GetWithDetailsAsync(item.Id);
            if (detailed is not null) result.Add(MapToDto(detailed));
        }
        return result;
    }

    public async Task<IEnumerable<PremiumMaintenanceRequestDto>> GetByClientIdAsync(Guid clientId)
    {
        var items = await _repo.GetByClientIdAsync(clientId);
        return items.Where(r => !r.IsDeleted).Select(MapToDto);
    }

    public async Task<PremiumMaintenanceRequestDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<PremiumMaintenanceRequestDto> CreateAsync(CreatePremiumMaintenanceRequestDto dto)
    {
        var entity = new PremiumMaintenanceRequest
        {
            ClientId = dto.ClientId,
            PremiumServiceId = dto.PremiumServiceId,
            ScheduledDate = dto.ScheduledDate,
            Notes = dto.Notes,
            Address = dto.Address,
            Status = PremiumMaintenanceStatus.PaymentPending
        };

        var created = await _repo.AddAsync(entity);
        var detailed = await _repo.GetWithDetailsAsync(created.Id);
        return MapToDto(detailed ?? created);
    }

    public async Task<PremiumMaintenanceRequestDto?> UpdateAsync(Guid id, UpdatePremiumMaintenanceRequestDto dto)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Status = dto.Status;
        item.ScheduledDate = dto.ScheduledDate;
        item.Notes = dto.Notes;
        item.Address = dto.Address;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        var updated = await _repo.GetWithDetailsAsync(id);
        return MapToDto(updated ?? item);
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

    private static PremiumMaintenanceRequestDto MapToDto(PremiumMaintenanceRequest r) => new()
    {
        Id = r.Id,
        ClientId = r.ClientId,
        ClientName = r.Client?.Name ?? string.Empty,
        PremiumServiceId = r.PremiumServiceId,
        ServiceName = r.PremiumService?.Name ?? string.Empty,
        ServicePrice = r.PremiumService?.Price ?? 0,
        ServiceType = r.PremiumService?.ServiceType ?? Maintenance_management.domain.Enums.PremiumServiceType.Preventive,
        Status = r.Status,
        RequestDate = r.RequestDate,
        ScheduledDate = r.ScheduledDate,
        Notes = r.Notes,
        Address = r.Address,
        PaymentStatus = r.Payment?.Status,
        TransactionId = r.Payment?.TransactionId,
        CreatedAt = r.CreatedAt
    };
}
