namespace Maintenance_management.application.DTOs.Report;

public class ReportDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string? BeforePhotoUrl { get; set; }
    public string? AfterPhotoUrl { get; set; }
    public string? PdfUrl { get; set; }
    public decimal? LaborHours { get; set; }
    public decimal? MaterialCost { get; set; }
    public string? Recommendations { get; set; }
    public Guid? TaskOrderId { get; set; }
    public string? TaskTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReportDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public string? BeforePhotoUrl { get; set; }
    public string? AfterPhotoUrl { get; set; }
    public decimal? LaborHours { get; set; }
    public decimal? MaterialCost { get; set; }
    public string? Recommendations { get; set; }
    public Guid? TaskOrderId { get; set; }
}

public class UpdateReportDto : CreateReportDto { }
