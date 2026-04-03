using Maintenance_management.application.DTOs.Technician;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class GpsTrackingService : IGpsTrackingService
{
    private readonly ITechnicianRepository _technicianRepo;
    private readonly ITechnicianGpsLogRepository _gpsLogRepo;

    public GpsTrackingService(ITechnicianRepository technicianRepo, ITechnicianGpsLogRepository gpsLogRepo)
    {
        _technicianRepo = technicianRepo;
        _gpsLogRepo = gpsLogRepo;
    }

    public async Task UpdateTechnicianLocationAsync(Guid technicianId, double latitude, double longitude)
    {
        var technician = await _technicianRepo.GetByIdAsync(technicianId);
        if (technician is null) return;

        technician.Latitude = latitude;
        technician.Longitude = longitude;
        technician.LastLocationUpdate = DateTime.UtcNow;
        technician.UpdatedAt = DateTime.UtcNow;
        await _technicianRepo.UpdateAsync(technician);

        await _gpsLogRepo.AddAsync(new TechnicianGpsLog
        {
            TechnicianId = technicianId,
            Latitude = latitude,
            Longitude = longitude,
            RecordedAt = DateTime.UtcNow
        });
    }

    public async Task<IEnumerable<TechnicianGpsLogDto>> GetLocationHistoryAsync(Guid technicianId)
    {
        var logs = await _gpsLogRepo.GetByTechnicianIdAsync(technicianId);
        return logs.Select(MapToDto);
    }

    public async Task<TechnicianGpsLogDto?> GetLatestLocationAsync(Guid technicianId)
    {
        var log = await _gpsLogRepo.GetLatestByTechnicianIdAsync(technicianId);
        return log is null ? null : MapToDto(log);
    }

    public async Task<TechnicianDistanceDto?> CalculateDistanceAsync(Guid technicianId, double serviceLatitude, double serviceLongitude)
    {
        var technician = await _technicianRepo.GetByIdAsync(technicianId);
        if (technician is null || !technician.Latitude.HasValue || !technician.Longitude.HasValue)
            return null;

        var distanceKm = HaversineDistanceKm(
            technician.Latitude.Value,
            technician.Longitude.Value,
            serviceLatitude,
            serviceLongitude);

        return new TechnicianDistanceDto
        {
            TechnicianId = technician.Id,
            TechnicianName = $"{technician.FirstName} {technician.LastName}",
            TechnicianLatitude = technician.Latitude.Value,
            TechnicianLongitude = technician.Longitude.Value,
            ServiceLatitude = serviceLatitude,
            ServiceLongitude = serviceLongitude,
            DistanceKm = distanceKm
        };
    }

    private static double HaversineDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

    private static TechnicianGpsLogDto MapToDto(TechnicianGpsLog log) => new()
    {
        Id = log.Id,
        TechnicianId = log.TechnicianId,
        Latitude = log.Latitude,
        Longitude = log.Longitude,
        RecordedAt = log.RecordedAt
    };
}
