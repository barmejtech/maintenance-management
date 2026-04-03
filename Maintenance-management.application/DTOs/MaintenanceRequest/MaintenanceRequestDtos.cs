using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.MaintenanceRequest;

public class MaintenanceRequestDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EquipmentDescription { get; set; }
    public DateTime RequestDate { get; set; }
    public MaintenanceRequestStatus Status { get; set; }
    public string? Notes { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public Guid? TaskOrderId { get; set; }
    public string? TaskTitle { get; set; }
    public Guid? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    // Review info
    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }

    // Assignment info
    public List<AssignedTechnicianDto> AssignedTechnicians { get; set; } = new();
}

public class AssignedTechnicianDto
{
    public Guid TechnicianId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string PerformedByName { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMaintenanceRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EquipmentDescription { get; set; }
    public DateTime? RequestDate { get; set; }
    public Guid ClientId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>Used by clients submitting their own requests — ClientId is resolved server-side from JWT.</summary>
public class SubmitMaintenanceRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EquipmentDescription { get; set; }
    public DateTime? RequestDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateMaintenanceRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EquipmentDescription { get; set; }
    public DateTime? RequestDate { get; set; }
    public MaintenanceRequestStatus Status { get; set; }
    public string? Notes { get; set; }
    public Guid? TaskOrderId { get; set; }
    public Guid? InvoiceId { get; set; }
}

public class ApproveMaintenanceRequestDto
{
    public string? ReviewNotes { get; set; }
}

public class RejectMaintenanceRequestDto
{
    public string? ReviewNotes { get; set; }
}

public class AssignTechniciansDto
{
    public List<Guid> TechnicianIds { get; set; } = new();
}
