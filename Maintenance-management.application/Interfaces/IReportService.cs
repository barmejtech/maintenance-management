using Maintenance_management.application.DTOs.Report;

namespace Maintenance_management.application.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReportDto>> GetAllAsync();
    Task<ReportDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ReportDto>> GetByTaskOrderIdAsync(Guid taskOrderId);
    Task<IEnumerable<ReportDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<ReportDto> CreateAsync(CreateReportDto dto, string createdByUserId);
    Task<ReportDto?> UpdateAsync(Guid id, UpdateReportDto dto);
    Task<bool> DeleteAsync(Guid id);
}
