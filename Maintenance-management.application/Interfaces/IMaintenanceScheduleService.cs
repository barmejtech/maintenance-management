using Maintenance_management.application.DTOs.MaintenanceSchedule;

namespace Maintenance_management.application.Interfaces;

public interface IMaintenanceScheduleService
{
    Task<IEnumerable<MaintenanceScheduleDto>> GetAllAsync();
    Task<MaintenanceScheduleDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<MaintenanceScheduleDto>> GetByEquipmentIdAsync(Guid equipmentId);
    Task<IEnumerable<MaintenanceScheduleDto>> GetActiveSchedulesAsync();
    Task<IEnumerable<MaintenanceScheduleDto>> GetOverdueSchedulesAsync();
    Task<IEnumerable<MaintenanceScheduleDto>> GetDueSoonAsync(int withinDays = 7);
    Task<MaintenanceScheduleDto> CreateAsync(CreateMaintenanceScheduleDto dto, string createdByUserId);
    Task<MaintenanceScheduleDto?> UpdateAsync(Guid id, UpdateMaintenanceScheduleDto dto);
    Task<bool> DeleteAsync(Guid id);
}
