using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.AdminDashboard;

/// <summary>One row in the admin dashboard table – one maintenance request.</summary>
public class AdminDashboardRowDto
{
    public Guid RequestId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime DateRequest { get; set; }

    /// <summary>Comma-separated list of assigned technician names.</summary>
    public string Technicians { get; set; } = string.Empty;

    /// <summary>Scheduled date from the linked task order (null when no task is linked).</summary>
    public DateTime? DateOfTask { get; set; }

    public MaintenanceRequestStatus Status { get; set; }

    /// <summary>Total invoice amount (null when no invoice is linked).</summary>
    public decimal? Price { get; set; }

    /// <summary>Whether the invoice has been paid.</summary>
    public bool IsPaid { get; set; }

    /// <summary>Date the invoice was paid (null when unpaid or no invoice).</summary>
    public DateTime? DatePay { get; set; }
}

/// <summary>Tasks completed on a specific calendar day.</summary>
public class DailyStatDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

/// <summary>Full payload returned by the admin dashboard endpoint.</summary>
public class AdminDashboardDto
{
    public IEnumerable<AdminDashboardRowDto> Rows { get; set; } = Enumerable.Empty<AdminDashboardRowDto>();

    /// <summary>Per-day breakdown of completed tasks (last 30 days).</summary>
    public IEnumerable<DailyStatDto> DailyStats { get; set; } = Enumerable.Empty<DailyStatDto>();

    /// <summary>Average number of completed tasks per week (over all recorded history).</summary>
    public double WeeklyAverage { get; set; }
}
