using Maintenance_management.application.DTOs.Invoice;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repo;

    public InvoiceService(IInvoiceRepository repo) => _repo = repo;

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(i => !i.IsDeleted).Select(MapToDto);
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithLineItemsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<InvoiceDto>> GetByStatusAsync(InvoiceStatus status)
    {
        var items = await _repo.GetByStatusAsync(status);
        return items.Where(i => !i.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<InvoiceDto>> GetByClientIdAsync(Guid clientId)
    {
        var items = await _repo.GetByClientIdAsync(clientId);
        return items.Where(i => !i.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<InvoiceDto>> GetByMaintenanceReportIdAsync(Guid reportId)
    {
        var items = await _repo.GetByMaintenanceReportIdAsync(reportId);
        return items.Where(i => !i.IsDeleted).Select(MapToDto);
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto, string createdByUserId)
    {
        var lineItems = dto.LineItems.Select(li => new InvoiceLineItem
        {
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            Total = li.Quantity * li.UnitPrice
        }).ToList();

        var subTotal = lineItems.Sum(li => li.Total);
        var taxAmount = subTotal * dto.TaxRate / 100;

        var entity = new Invoice
        {
            InvoiceNumber = GenerateInvoiceNumber(),
            ClientName = dto.ClientName,
            ClientEmail = dto.ClientEmail,
            ClientAddress = dto.ClientAddress,
            DueDate = dto.DueDate,
            SubTotal = subTotal,
            TaxRate = dto.TaxRate,
            TaxAmount = taxAmount,
            TotalAmount = subTotal + taxAmount,
            Notes = dto.Notes,
            TaskOrderId = dto.TaskOrderId,
            ClientId = dto.ClientId,
            MaintenanceReportId = dto.MaintenanceReportId,
            CreatedByUserId = createdByUserId,
            LineItems = lineItems
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<InvoiceDto?> UpdateAsync(Guid id, UpdateInvoiceDto dto)
    {
        var item = await _repo.GetWithLineItemsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.ClientName = dto.ClientName;
        item.ClientEmail = dto.ClientEmail;
        item.ClientAddress = dto.ClientAddress;
        item.DueDate = dto.DueDate;
        item.Notes = dto.Notes;
        item.Status = dto.Status;
        item.TaskOrderId = dto.TaskOrderId;
        item.ClientId = dto.ClientId;
        item.MaintenanceReportId = dto.MaintenanceReportId;
        item.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == InvoiceStatus.Paid && item.PaidDate is null)
            item.PaidDate = DateTime.UtcNow;

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

    private static string GenerateInvoiceNumber()
        => $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

    private static InvoiceDto MapToDto(Invoice inv) => new()
    {
        Id = inv.Id,
        InvoiceNumber = inv.InvoiceNumber,
        ClientName = inv.ClientName,
        ClientEmail = inv.ClientEmail,
        ClientAddress = inv.ClientAddress,
        IssueDate = inv.IssueDate,
        DueDate = inv.DueDate,
        PaidDate = inv.PaidDate,
        SubTotal = inv.SubTotal,
        TaxRate = inv.TaxRate,
        TaxAmount = inv.TaxAmount,
        TotalAmount = inv.TotalAmount,
        Status = inv.Status,
        Notes = inv.Notes,
        TaskOrderId = inv.TaskOrderId,
        TaskTitle = inv.TaskOrder?.Title,
        ClientId = inv.ClientId,
        ClientCompany = inv.Client?.CompanyName ?? inv.Client?.Name,
        MaintenanceReportId = inv.MaintenanceReportId,
        MaintenanceReportTitle = inv.MaintenanceReport?.Title,
        LineItems = inv.LineItems.Select(li => new InvoiceLineItemDto
        {
            Id = li.Id,
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            Total = li.Total
        }).ToList(),
        CreatedAt = inv.CreatedAt
    };
}
