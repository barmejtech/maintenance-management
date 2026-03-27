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
