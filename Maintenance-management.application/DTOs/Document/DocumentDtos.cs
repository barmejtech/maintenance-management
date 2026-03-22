namespace Maintenance_management.application.DTOs.Document;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string UploadedByUserId { get; set; } = string.Empty;
    public Guid? TechnicianId { get; set; }
    public Guid? TaskOrderId { get; set; }
    public Guid? EquipmentId { get; set; }
    public DateTime CreatedAt { get; set; }
}
