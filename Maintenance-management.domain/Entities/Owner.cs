namespace Maintenance_management.domain.Entities;

public class Owner : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public ICollection<UnitOwnership> UnitOwnerships { get; set; } = new List<UnitOwnership>();

    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}