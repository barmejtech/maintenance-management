using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByTechnicianIdAsync(Guid technicianId);
    Task<IEnumerable<Document>> GetByTaskOrderIdAsync(Guid taskOrderId);
    Task<IEnumerable<Document>> GetByEquipmentIdAsync(Guid equipmentId);
}
