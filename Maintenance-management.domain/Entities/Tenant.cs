namespace Maintenance_management.domain.Entities;

public class Tenant : BaseEntity
{
    public Guid UnitId { get; set; }
    public Unit? Unit { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }

    public DateTime LeaseStartDate { get; set; }
    public DateTime LeaseEndDate { get; set; }
    public decimal RentalAmount { get; set; }
    public decimal? DepositAmount { get; set; }

    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}