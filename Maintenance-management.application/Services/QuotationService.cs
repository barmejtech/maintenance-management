using Maintenance_management.application.DTOs.Quotation;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class QuotationService : IQuotationService
{
    private readonly IQuotationRepository _repo;

    public QuotationService(IQuotationRepository repo) => _repo = repo;

    public async Task<IEnumerable<QuotationDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(q => !q.IsDeleted).Select(MapToDto);
    }

    public async Task<QuotationDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithLineItemsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<QuotationDto>> GetByStatusAsync(QuotationStatus status)
    {
        var items = await _repo.GetByStatusAsync(status);
        return items.Where(q => !q.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<QuotationDto>> GetByClientIdAsync(Guid clientId)
    {
        var items = await _repo.GetByClientIdAsync(clientId);
        return items.Where(q => !q.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<QuotationDto>> GetByMaintenanceRequestIdAsync(Guid requestId)
    {
        var items = await _repo.GetByMaintenanceRequestIdAsync(requestId);
        return items.Where(q => !q.IsDeleted).Select(MapToDto);
    }

    public async Task<QuotationDto> CreateAsync(CreateQuotationDto dto, string createdByUserId)
    {
        var lineItems = dto.LineItems.Select(li => new QuotationLineItem
        {
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            Total = li.Quantity * li.UnitPrice
        }).ToList();

        var subTotal = lineItems.Sum(li => li.Total);
        var taxAmount = subTotal * dto.TaxRate / 100;

        var entity = new Quotation
        {
            QuotationNumber = GenerateQuotationNumber(),
            ClientName = dto.ClientName,
            ClientEmail = dto.ClientEmail,
            ClientAddress = dto.ClientAddress,
            ClientPhone = dto.ClientPhone,
            ValidUntil = dto.ValidUntil ?? DateTime.UtcNow.AddDays(30),
            EstimatedDurationDays = dto.EstimatedDurationDays,
            SubTotal = subTotal,
            TaxRate = dto.TaxRate,
            TaxAmount = taxAmount,
            TotalAmount = subTotal + taxAmount,
            Notes = dto.Notes,
            TermsAndConditions = dto.TermsAndConditions,
            MaintenanceRequestId = dto.MaintenanceRequestId,
            ClientId = dto.ClientId,
            CreatedByUserId = createdByUserId,
            LineItems = lineItems
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<QuotationDto?> UpdateAsync(Guid id, UpdateQuotationDto dto)
    {
        var item = await _repo.GetWithLineItemsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.ClientName = dto.ClientName;
        item.ClientEmail = dto.ClientEmail;
        item.ClientAddress = dto.ClientAddress;
        item.ClientPhone = dto.ClientPhone;
        item.ValidUntil = dto.ValidUntil ?? item.ValidUntil;
        item.EstimatedDurationDays = dto.EstimatedDurationDays;
        item.Notes = dto.Notes;
        item.TermsAndConditions = dto.TermsAndConditions;
        item.Status = dto.Status;
        item.MaintenanceRequestId = dto.MaintenanceRequestId;
        item.ClientId = dto.ClientId;
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

    private static string GenerateQuotationNumber()
        => $"QTN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

    private static QuotationDto MapToDto(Quotation q) => new()
    {
        Id = q.Id,
        QuotationNumber = q.QuotationNumber,
        ClientName = q.ClientName,
        ClientEmail = q.ClientEmail,
        ClientAddress = q.ClientAddress,
        ClientPhone = q.ClientPhone,
        IssueDate = q.IssueDate,
        ValidUntil = q.ValidUntil,
        EstimatedDurationDays = q.EstimatedDurationDays,
        SubTotal = q.SubTotal,
        TaxRate = q.TaxRate,
        TaxAmount = q.TaxAmount,
        TotalAmount = q.TotalAmount,
        Status = q.Status,
        Notes = q.Notes,
        TermsAndConditions = q.TermsAndConditions,
        MaintenanceRequestId = q.MaintenanceRequestId,
        MaintenanceRequestTitle = q.MaintenanceRequest?.Title,
        ClientId = q.ClientId,
        ClientCompany = q.Client?.CompanyName ?? q.Client?.Name,
        LineItems = q.LineItems.Select(li => new QuotationLineItemDto
        {
            Id = li.Id,
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            Total = li.Total
        }).ToList(),
        CreatedAt = q.CreatedAt
    };
}
