using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, string entityId)
        => await _dbSet
            .Where(l => l.EntityType == entityType && l.EntityId == entityId && !l.IsDeleted)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
}
