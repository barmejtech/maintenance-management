using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetByTechnicianIdAsync(Guid technicianId)
        => await _dbSet.Where(d => d.TechnicianId == technicianId && !d.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Document>> GetByTaskOrderIdAsync(Guid taskOrderId)
        => await _dbSet.Where(d => d.TaskOrderId == taskOrderId && !d.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Document>> GetByEquipmentIdAsync(Guid equipmentId)
        => await _dbSet.Where(d => d.EquipmentId == equipmentId && !d.IsDeleted).ToListAsync();
}
