using Maintenance_management.application.DTOs.AdminDashboard;

namespace Maintenance_management.application.Interfaces;

public interface IAdminDashboardService
{
    /// <summary>Returns the full admin dashboard payload.</summary>
    Task<AdminDashboardDto> GetDashboardAsync();
}
