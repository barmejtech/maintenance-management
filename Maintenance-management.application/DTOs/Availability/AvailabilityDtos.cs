namespace Maintenance_management.application.DTOs.Availability;

public class AvailabilityDto
{
    public Guid Id { get; set; }
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAvailabilityDto
{
    public Guid TechnicianId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateAvailabilityDto : CreateAvailabilityDto { }
