using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Interfaces;

public interface IMeterReadingRepository : IRepository<MeterReading>
{
    Task<IEnumerable<MeterReading>> GetByUnitIdAsync(Guid unitId);
    Task<IEnumerable<MeterReading>> GetByTypeAsync(MeterType type);
    Task<MeterReading?> GetLatestByUnitAndTypeAsync(Guid unitId, MeterType type);
    Task<IEnumerable<MeterReading>> GetByUnitAndTypeWithDateRangeAsync(Guid unitId, MeterType type, int months);
    Task<IEnumerable<MeterReading>> GetByUnitIdAndDateRangeAsync(Guid unitId, DateTime start, DateTime end);
}