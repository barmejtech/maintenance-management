using Maintenance_management.application.DTOs.TechnicianPerformance;

namespace Maintenance_management.application.Interfaces;

public interface ITechnicianPerformanceService
{
    Task<IEnumerable<TechnicianPerformanceScoreDto>> GetAllAsync();
    Task<TechnicianPerformanceScoreDto?> GetByTechnicianIdAsync(Guid technicianId);
    Task<IEnumerable<TechnicianPerformanceScoreDto>> GetTopPerformersAsync(int count = 10);
    Task<TechnicianPerformanceScoreDto> RecalculateAsync(Guid technicianId);
    Task<bool> UpdateCustomerSatisfactionAsync(Guid technicianId, double score);
    Task<SmartDispatchResultDto?> GetBestTechnicianForTaskAsync(Guid taskOrderId);
}
