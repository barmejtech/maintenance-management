using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class Equipment : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Operational;
    public string? Notes { get; set; }

    public ICollection<TaskOrder> Tasks { get; set; } = new List<TaskOrder>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public HVACSystem? HVACSystem { get; set; }
}
