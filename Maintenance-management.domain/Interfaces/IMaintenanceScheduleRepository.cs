using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IMaintenanceScheduleRepository : IRepository<MaintenanceSchedule>
{
    Task<IEnumerable<MaintenanceSchedule>> GetByEquipmentIdAsync(Guid equipmentId);
    Task<IEnumerable<MaintenanceSchedule>> GetActiveSchedulesAsync();
    Task<IEnumerable<MaintenanceSchedule>> GetOverdueSchedulesAsync();
    Task<IEnumerable<MaintenanceSchedule>> GetDueSoonAsync(int withinDays = 7);
}
