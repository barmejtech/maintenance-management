using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IEquipmentHealthPredictionRepository : IRepository<EquipmentHealthPrediction>
{
    Task<EquipmentHealthPrediction?> GetByEquipmentIdAsync(Guid equipmentId);
    Task<IEnumerable<EquipmentHealthPrediction>> GetHighRiskAsync(double threshold);
}
