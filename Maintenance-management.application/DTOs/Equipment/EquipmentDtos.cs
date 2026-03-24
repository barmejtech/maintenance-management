using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.Equipment;

public class EquipmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public EquipmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? BeforeMaintenancePhotoUrl { get; set; }
    public string? AfterMaintenancePhotoUrl { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEquipmentDto
{
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string? Notes { get; set; }
    public string? BeforeMaintenancePhotoUrl { get; set; }
    public string? AfterMaintenancePhotoUrl { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
}

public class UpdateEquipmentDto
{
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public EquipmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? BeforeMaintenancePhotoUrl { get; set; }
    public string? AfterMaintenancePhotoUrl { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
}
