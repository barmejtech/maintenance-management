using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IReportRepository : IRepository<MaintenanceReport>
{
    Task<IEnumerable<MaintenanceReport>> GetByTaskOrderIdAsync(Guid taskOrderId);
    Task<IEnumerable<MaintenanceReport>> GetByTechnicianNameAsync(string name);
    Task<IEnumerable<MaintenanceReport>> GetByDateRangeAsync(DateTime from, DateTime to);
}
