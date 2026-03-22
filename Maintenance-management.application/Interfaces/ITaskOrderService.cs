using Maintenance_management.application.DTOs.TaskOrder;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.Interfaces;

public interface ITaskOrderService
{
    Task<IEnumerable<TaskOrderDto>> GetAllAsync();
    Task<TaskOrderDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskOrderDto>> GetByTechnicianIdAsync(Guid technicianId);
    Task<IEnumerable<TaskOrderDto>> GetByGroupIdAsync(Guid groupId);
    Task<IEnumerable<TaskOrderDto>> GetByStatusAsync(domain.Enums.TaskStatus status);
    Task<IEnumerable<TaskOrderDto>> GetScheduledBetweenAsync(DateTime from, DateTime to);
    Task<TaskOrderDto> CreateAsync(CreateTaskOrderDto dto, string createdByUserId);
    Task<TaskOrderDto?> UpdateAsync(Guid id, UpdateTaskOrderDto dto);
    Task<bool> DeleteAsync(Guid id);
}
