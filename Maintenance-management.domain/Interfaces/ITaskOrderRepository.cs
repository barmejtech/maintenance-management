using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Interfaces;

public interface ITaskOrderRepository : IRepository<TaskOrder>
{
    Task<IEnumerable<TaskOrder>> GetByTechnicianIdAsync(Guid technicianId);
    Task<IEnumerable<TaskOrder>> GetByGroupIdAsync(Guid groupId);
    Task<IEnumerable<TaskOrder>> GetByStatusAsync(Enums.TaskStatus status);
    Task<IEnumerable<TaskOrder>> GetByEquipmentIdAsync(Guid equipmentId);
    Task<IEnumerable<TaskOrder>> GetScheduledBetweenAsync(DateTime from, DateTime to);
    Task<TaskOrder?> GetWithDetailsAsync(Guid id);
}
