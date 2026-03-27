namespace Maintenance_management.domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }

    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
