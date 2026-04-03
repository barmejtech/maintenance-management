using Maintenance_management.application.DTOs.TravelEstimation;

namespace Maintenance_management.application.Interfaces;

public interface ITravelEstimationService
{
    /// <summary>
    /// Calculates the driving distance and estimated travel time from the
    /// technician's registered GPS coordinates to the client's address.
    /// </summary>
    Task<TravelEstimationResultDto?> EstimateAsync(Guid technicianId, Guid clientId);
}
