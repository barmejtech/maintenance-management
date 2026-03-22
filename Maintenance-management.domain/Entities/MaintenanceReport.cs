namespace Maintenance_management.domain.Entities;

public class MaintenanceReport : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public string? BeforePhotoUrl { get; set; }
    public string? AfterPhotoUrl { get; set; }
    public string? PdfUrl { get; set; }
    public decimal? LaborHours { get; set; }
    public decimal? MaterialCost { get; set; }
    public string? Recommendations { get; set; }

    public Guid? TaskOrderId { get; set; }
    public TaskOrder? TaskOrder { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
