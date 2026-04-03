using Maintenance_management.application.DTOs.MaintenanceRequest;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.Interfaces;

public interface IMaintenanceRequestService
{
    Task<IEnumerable<MaintenanceRequestDto>> GetAllAsync();
    Task<MaintenanceRequestDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<MaintenanceRequestDto>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<MaintenanceRequestDto>> GetByStatusAsync(MaintenanceRequestStatus status);
    Task<MaintenanceRequestDto> CreateAsync(CreateMaintenanceRequestDto dto);
    Task<MaintenanceRequestDto?> UpdateAsync(Guid id, UpdateMaintenanceRequestDto dto);
    Task<bool> DeleteAsync(Guid id);

    /// <summary>Approves a pending maintenance request.</summary>
    Task<MaintenanceRequestDto?> ApproveAsync(Guid id, string reviewedByUserId, string reviewedByName, string? reviewNotes);

    /// <summary>Rejects a pending maintenance request.</summary>
    Task<MaintenanceRequestDto?> RejectAsync(Guid id, string reviewedByUserId, string reviewedByName, string? reviewNotes);

    /// <summary>Assigns one or more technicians to an approved request.</summary>
    Task<MaintenanceRequestDto?> AssignTechniciansAsync(Guid id, List<Guid> technicianIds, string assignedByUserId);

    /// <summary>Returns the audit log for a specific request.</summary>
    Task<IEnumerable<AuditLogDto>> GetAuditLogAsync(Guid id);
}
