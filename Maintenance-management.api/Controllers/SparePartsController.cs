using Maintenance_management.application.DTOs.SparePart;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SparePartsController : ControllerBase
{
    private readonly ISparePartService _service;
    private readonly INotificationService _notificationService;

    public SparePartsController(ISparePartService service, INotificationService notificationService)
    {
        _service = service;
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
        => Ok(await _service.GetLowStockAsync());

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateSparePartDto dto)
    {
        var result = await _service.CreateAsync(dto);

        // Warn if newly added part is already at or below minimum stock level
        if (result.IsLowStock)
        {
            await _notificationService.SendToRoleAsync("Admin",
                "Low Stock Alert",
                $"Spare part \"{result.Name}\" (#{result.PartNumber}) is at or below minimum stock level ({result.QuantityInStock}/{result.MinimumStockLevel}).",
                "warning", result.Id.ToString(), "SparePart");
            await _notificationService.SendToRoleAsync("Manager",
                "Low Stock Alert",
                $"Spare part \"{result.Name}\" (#{result.PartNumber}) is at or below minimum stock level ({result.QuantityInStock}/{result.MinimumStockLevel}).",
                "warning", result.Id.ToString(), "SparePart");
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSparePartDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result is null) return NotFound();

        // Notify if updated stock is at or below minimum
        if (result.IsLowStock)
        {
            await _notificationService.SendToRoleAsync("Admin",
                "Low Stock Alert",
                $"Spare part \"{result.Name}\" (#{result.PartNumber}) is at or below minimum stock level ({result.QuantityInStock}/{result.MinimumStockLevel}).",
                "warning", result.Id.ToString(), "SparePart");
            await _notificationService.SendToRoleAsync("Manager",
                "Low Stock Alert",
                $"Spare part \"{result.Name}\" (#{result.PartNumber}) is at or below minimum stock level ({result.QuantityInStock}/{result.MinimumStockLevel}).",
                "warning", result.Id.ToString(), "SparePart");
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/usages")]
    public async Task<IActionResult> GetUsages(Guid id)
        => Ok(await _service.GetUsagesBySparePartIdAsync(id));

    [HttpPost("usages")]
    public async Task<IActionResult> AddUsage([FromBody] CreateSparePartUsageDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _service.AddUsageAsync(dto, userId);

            // Check if the spare part is now low on stock after usage
            var sparePart = await _service.GetByIdAsync(dto.SparePartId);
            if (sparePart?.IsLowStock == true)
            {
                await _notificationService.SendToRoleAsync("Admin",
                    "Low Stock Alert",
                    $"Spare part \"{sparePart.Name}\" (#{sparePart.PartNumber}) is at or below minimum stock level ({sparePart.QuantityInStock}/{sparePart.MinimumStockLevel}).",
                    "warning", sparePart.Id.ToString(), "SparePart");
                await _notificationService.SendToRoleAsync("Manager",
                    "Low Stock Alert",
                    $"Spare part \"{sparePart.Name}\" (#{sparePart.PartNumber}) is at or below minimum stock level ({sparePart.QuantityInStock}/{sparePart.MinimumStockLevel}).",
                    "warning", sparePart.Id.ToString(), "SparePart");
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("usages/{usageId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteUsage(Guid usageId)
    {
        var result = await _service.DeleteUsageAsync(usageId);
        return result ? NoContent() : NotFound();
    }
}
