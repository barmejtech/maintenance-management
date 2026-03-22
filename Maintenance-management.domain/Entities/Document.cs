namespace Maintenance_management.domain.Entities;

public class Document : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string UploadedByUserId { get; set; } = string.Empty;

    public Guid? TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public Guid? TaskOrderId { get; set; }
    public TaskOrder? TaskOrder { get; set; }

    public Guid? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public Guid? MaintenanceReportId { get; set; }
    public MaintenanceReport? MaintenanceReport { get; set; }

    public Guid? HVACSystemId { get; set; }
    public HVACSystem? HVACSystem { get; set; }
}
