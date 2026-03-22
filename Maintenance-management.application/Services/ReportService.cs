using Maintenance_management.application.DTOs.Report;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;

    public ReportService(IReportRepository repo) => _repo = repo;

    public async Task<IEnumerable<ReportDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(r => !r.IsDeleted).Select(MapToDto);
    }

    public async Task<ReportDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<ReportDto>> GetByTaskOrderIdAsync(Guid taskOrderId)
    {
        var items = await _repo.GetByTaskOrderIdAsync(taskOrderId);
        return items.Where(r => !r.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<ReportDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var items = await _repo.GetByDateRangeAsync(from, to);
        return items.Where(r => !r.IsDeleted).Select(MapToDto);
    }

    public async Task<ReportDto> CreateAsync(CreateReportDto dto, string createdByUserId)
    {
        var entity = new MaintenanceReport
        {
            Title = dto.Title,
            Content = dto.Content,
            TechnicianName = dto.TechnicianName,
            ReportDate = dto.ReportDate,
            BeforePhotoUrl = dto.BeforePhotoUrl,
            AfterPhotoUrl = dto.AfterPhotoUrl,
            LaborHours = dto.LaborHours,
            MaterialCost = dto.MaterialCost,
            Recommendations = dto.Recommendations,
            TaskOrderId = dto.TaskOrderId,
            CreatedByUserId = createdByUserId
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<ReportDto?> UpdateAsync(Guid id, UpdateReportDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Title = dto.Title;
        item.Content = dto.Content;
        item.TechnicianName = dto.TechnicianName;
        item.ReportDate = dto.ReportDate;
        item.BeforePhotoUrl = dto.BeforePhotoUrl;
        item.AfterPhotoUrl = dto.AfterPhotoUrl;
        item.LaborHours = dto.LaborHours;
        item.MaterialCost = dto.MaterialCost;
        item.Recommendations = dto.Recommendations;
        item.TaskOrderId = dto.TaskOrderId;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return false;

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(item);
        return true;
    }

    private static ReportDto MapToDto(MaintenanceReport r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Content = r.Content,
        TechnicianName = r.TechnicianName,
        CreatedByUserId = r.CreatedByUserId,
        ReportDate = r.ReportDate,
        BeforePhotoUrl = r.BeforePhotoUrl,
        AfterPhotoUrl = r.AfterPhotoUrl,
        PdfUrl = r.PdfUrl,
        LaborHours = r.LaborHours,
        MaterialCost = r.MaterialCost,
        Recommendations = r.Recommendations,
        TaskOrderId = r.TaskOrderId,
        TaskTitle = r.TaskOrder?.Title,
        CreatedAt = r.CreatedAt
    };
}
