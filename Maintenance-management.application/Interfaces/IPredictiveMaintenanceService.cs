using Maintenance_management.application.DTOs.PredictiveMaintenance;

namespace Maintenance_management.application.Interfaces;

public interface IPredictiveMaintenanceService
{
    Task<IEnumerable<EquipmentHealthPredictionDto>> GetAllPredictionsAsync();
    Task<EquipmentHealthPredictionDto?> GetPredictionByEquipmentAsync(Guid equipmentId);
    Task<EquipmentHealthPredictionDto> RunPredictionAsync(Guid equipmentId);
    Task<IEnumerable<EquipmentHealthPredictionDto>> GetHighRiskEquipmentAsync(double threshold = 0.7);
}
