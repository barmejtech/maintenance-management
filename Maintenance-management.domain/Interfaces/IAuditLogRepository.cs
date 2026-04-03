using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, string entityId);
}
