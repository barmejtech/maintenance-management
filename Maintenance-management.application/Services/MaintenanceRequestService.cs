using Maintenance_management.application.DTOs.MaintenanceRequest;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class MaintenanceRequestService : IMaintenanceRequestService
{
    private readonly IMaintenanceRequestRepository _repo;
    private readonly IAuditLogRepository _auditRepo;

    public MaintenanceRequestService(IMaintenanceRequestRepository repo, IAuditLogRepository auditRepo)
    {
        _repo = repo;
        _auditRepo = auditRepo;
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(r => !r.IsDeleted).Select(MapToDto);
    }

    public async Task<MaintenanceRequestDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetByClientIdAsync(Guid clientId)
    {
        var items = await _repo.GetByClientIdAsync(clientId);
        return items.Where(r => !r.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetByStatusAsync(MaintenanceRequestStatus status)
    {
        var items = await _repo.GetByStatusAsync(status);
        return items.Where(r => !r.IsDeleted).Select(MapToDto);
    }

    public async Task<MaintenanceRequestDto> CreateAsync(CreateMaintenanceRequestDto dto)
    {
        var entity = new MaintenanceRequest
        {
            Title = dto.Title,
            Description = dto.Description,
            EquipmentDescription = dto.EquipmentDescription,
            RequestDate = dto.RequestDate ?? DateTime.UtcNow,
            ClientId = dto.ClientId,
            Notes = dto.Notes,
            Status = MaintenanceRequestStatus.Pending
        };

        var created = await _repo.AddAsync(entity);
        // reload with relations
        var detailed = await _repo.GetWithDetailsAsync(created.Id);
        return MapToDto(detailed ?? created);
    }

    public async Task<MaintenanceRequestDto?> UpdateAsync(Guid id, UpdateMaintenanceRequestDto dto)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Title = dto.Title;
        item.Description = dto.Description;
        item.EquipmentDescription = dto.EquipmentDescription;
        item.RequestDate = dto.RequestDate ?? item.RequestDate;
        item.Status = dto.Status;
        item.Notes = dto.Notes;
        item.TaskOrderId = dto.TaskOrderId;
        item.InvoiceId = dto.InvoiceId;
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

    public async Task<MaintenanceRequestDto?> ApproveAsync(Guid id, string reviewedByUserId, string reviewedByName, string? reviewNotes)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Status = MaintenanceRequestStatus.Approved;
        item.ReviewedByUserId = reviewedByUserId;
        item.ReviewedAt = DateTime.UtcNow;
        item.ReviewNotes = reviewNotes;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);

        // Audit log
        await _auditRepo.AddAsync(new AuditLog
        {
            EntityType = "MaintenanceRequest",
            EntityId = id.ToString(),
            Action = "Approved",
            PerformedByUserId = reviewedByUserId,
            PerformedByName = reviewedByName,
            Details = reviewNotes
        });

        var updated = await _repo.GetWithDetailsAsync(id);
        return MapToDto(updated ?? item);
    }

    public async Task<MaintenanceRequestDto?> RejectAsync(Guid id, string reviewedByUserId, string reviewedByName, string? reviewNotes)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Status = MaintenanceRequestStatus.Rejected;
        item.ReviewedByUserId = reviewedByUserId;
        item.ReviewedAt = DateTime.UtcNow;
        item.ReviewNotes = reviewNotes;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);

        // Audit log
        await _auditRepo.AddAsync(new AuditLog
        {
            EntityType = "MaintenanceRequest",
            EntityId = id.ToString(),
            Action = "Rejected",
            PerformedByUserId = reviewedByUserId,
            PerformedByName = reviewedByName,
            Details = reviewNotes
        });

        var updated = await _repo.GetWithDetailsAsync(id);
        return MapToDto(updated ?? item);
    }

    public async Task<MaintenanceRequestDto?> AssignTechniciansAsync(Guid id, List<Guid> technicianIds, string assignedByUserId)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        // Remove existing assignments
        await _repo.RemoveAssignmentsAsync(id);

        // Add new assignments
        foreach (var techId in technicianIds)
        {
            await _repo.AddAssignmentAsync(new MaintenanceRequestAssignment
            {
                MaintenanceRequestId = id,
                TechnicianId = techId,
                AssignedByUserId = assignedByUserId,
                AssignedAt = DateTime.UtcNow
            });
        }

        // Update status if still Approved
        if (item.Status == MaintenanceRequestStatus.Approved)
        {
            item.Status = MaintenanceRequestStatus.InProgress;
            item.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(item);
        }

        // Audit log
        await _auditRepo.AddAsync(new AuditLog
        {
            EntityType = "MaintenanceRequest",
            EntityId = id.ToString(),
            Action = "TechniciansAssigned",
            PerformedByUserId = assignedByUserId,
            PerformedByName = assignedByUserId,
            Details = $"Assigned {technicianIds.Count} technician(s): {string.Join(", ", technicianIds)}"
        });

        var updated = await _repo.GetWithDetailsAsync(id);
        return MapToDto(updated ?? item);
    }

    public async Task<IEnumerable<AuditLogDto>> GetAuditLogAsync(Guid id)
    {
        var logs = await _auditRepo.GetByEntityAsync("MaintenanceRequest", id.ToString());
        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id,
            Action = l.Action,
            PerformedByName = l.PerformedByName,
            Details = l.Details,
            CreatedAt = l.CreatedAt
        });
    }

    private static MaintenanceRequestDto MapToDto(MaintenanceRequest r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Description = r.Description,
        EquipmentDescription = r.EquipmentDescription,
        RequestDate = r.RequestDate,
        Status = r.Status,
        Notes = r.Notes,
        ClientId = r.ClientId,
        ClientName = r.Client?.Name ?? string.Empty,
        TaskOrderId = r.TaskOrderId,
        TaskTitle = r.TaskOrder?.Title,
        InvoiceId = r.InvoiceId,
        InvoiceNumber = r.Invoice?.InvoiceNumber,
        CreatedAt = r.CreatedAt,
        ReviewedByUserId = r.ReviewedByUserId,
        ReviewedAt = r.ReviewedAt,
        ReviewNotes = r.ReviewNotes,
        AssignedTechnicians = r.Assignments
            .Where(a => !a.IsDeleted && a.Technician != null)
            .Select(a => new AssignedTechnicianDto
            {
                TechnicianId = a.TechnicianId,
                FullName = $"{a.Technician!.FirstName} {a.Technician.LastName}",
                Specialization = a.Technician.Specialization,
                Email = a.Technician.Email,
                AssignedAt = a.AssignedAt
            }).ToList()
    };
}
