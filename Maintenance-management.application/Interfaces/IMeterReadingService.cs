using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.Interfaces;

public interface IMeterReadingService
{
    Task<IEnumerable<MeterReadingDto>> GetAllAsync();
    Task<MeterReadingDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<MeterReadingDto>> GetByUnitIdAsync(Guid unitId);
    Task<IEnumerable<MeterReadingDto>> GetByTypeAsync(MeterType type);
    Task<MeterReadingDto> CreateAsync(CreateMeterReadingDto dto, string readByUserId);
    Task<MeterReadingDto?> UpdateAsync(Guid id, UpdateMeterReadingDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<MeterReadingChartDataDto> GetChartDataAsync(Guid unitId, MeterType type, int months = 12);
    Task<BulkMeterReadingResultDto> BulkCreateAsync(BulkMeterReadingDto dto, string readByUserId);
}