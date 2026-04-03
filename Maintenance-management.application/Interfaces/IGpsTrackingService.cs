using Maintenance_management.application.DTOs.Technician;

namespace Maintenance_management.application.Interfaces;

public interface IGpsTrackingService
{
    Task UpdateTechnicianLocationAsync(Guid technicianId, double latitude, double longitude);
    Task<IEnumerable<TechnicianGpsLogDto>> GetLocationHistoryAsync(Guid technicianId);
    Task<TechnicianGpsLogDto?> GetLatestLocationAsync(Guid technicianId);
    Task<TechnicianDistanceDto?> CalculateDistanceAsync(Guid technicianId, double serviceLatitude, double serviceLongitude);
}
