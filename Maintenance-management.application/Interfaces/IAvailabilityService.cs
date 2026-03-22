using Maintenance_management.application.DTOs.Availability;

namespace Maintenance_management.application.Interfaces;

public interface IAvailabilityService
{
    Task<IEnumerable<AvailabilityDto>> GetAllAsync();
    Task<AvailabilityDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AvailabilityDto>> GetByTechnicianIdAsync(Guid technicianId);
    Task<IEnumerable<AvailabilityDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<AvailabilityDto> CreateAsync(CreateAvailabilityDto dto);
    Task<AvailabilityDto?> UpdateAsync(Guid id, UpdateAvailabilityDto dto);
    Task<bool> DeleteAsync(Guid id);
}
