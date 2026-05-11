using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repo;
    private readonly IVendorRepository _vendorRepo;

    public ExpenseService(IExpenseRepository repo, IVendorRepository vendorRepo)
    {
        _repo = repo;
        _vendorRepo = vendorRepo;
    }

    public async Task<IEnumerable<ExpenseDto>> GetAllAsync()
    {
        var items = await _repo.GetAllWithDetailsAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<ExpenseDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<ExpenseDto>> GetByVendorIdAsync(Guid vendorId)
    {
        var items = await _repo.GetByVendorIdAsync(vendorId);
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<ExpenseDto>> GetByStatusAsync(ExpenseStatus status)
    {
        var items = await _repo.GetByStatusAsync(status);
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto, string createdByUserId)
    {
        if (!await _vendorRepo.ExistsAsync(dto.VendorId))
            throw new KeyNotFoundException("Vendor not found.");

        var totalAmount = dto.Amount + (dto.TaxAmount ?? 0);

        var entity = new Expense
        {
            ExpenseNumber = GenerateExpenseNumber(),
            VendorId = dto.VendorId,
            Amount = dto.Amount,
            TaxAmount = dto.TaxAmount,
            TotalAmount = totalAmount,
            ExpenseDate = dto.ExpenseDate,
            DueDate = dto.DueDate,
            Description = dto.Description,
            InvoiceNumber = dto.InvoiceNumber,
            RenovationId = dto.RenovationId,
            SparePartId = dto.SparePartId,
            Status = ExpenseStatus.Draft,
            CreatedByUserId = createdByUserId
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<ExpenseDto?> UpdateAsync(Guid id, UpdateExpenseDto dto)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        var totalAmount = dto.Amount + (dto.TaxAmount ?? 0);

        item.Amount = dto.Amount;
        item.TaxAmount = dto.TaxAmount;
        item.TotalAmount = totalAmount;
        item.ExpenseDate = dto.ExpenseDate;
        item.DueDate = dto.DueDate;
        item.Status = dto.Status;
        item.Description = dto.Description;
        item.InvoiceNumber = dto.InvoiceNumber;

        if (dto.Status == ExpenseStatus.Paid && item.PaidDate is null)
            item.PaidDate = DateTime.UtcNow;

        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<ExpenseDto?> ApproveAsync(Guid id, string approvedByUserId)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Status = ExpenseStatus.Approved;
        item.ApprovedByUserId = approvedByUserId;
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

    private static string GenerateExpenseNumber()
        => $"EXP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

    private static ExpenseDto MapToDto(Expense e) => new()
    {
        Id = e.Id,
        ExpenseNumber = e.ExpenseNumber,
        VendorId = e.VendorId,
        VendorName = e.Vendor?.Name ?? string.Empty,
        Amount = e.Amount,
        TaxAmount = e.TaxAmount,
        TotalAmount = e.TotalAmount,
        ExpenseDate = e.ExpenseDate,
        DueDate = e.DueDate,
        PaidDate = e.PaidDate,
        Status = e.Status,
        Description = e.Description,
        InvoiceNumber = e.InvoiceNumber,
        RenovationId = e.RenovationId,
        RenovationTitle = e.Renovation?.Title,
        SparePartId = e.SparePartId,
        SparePartName = e.SparePart?.Name,
        CreatedAt = e.CreatedAt
    };
}