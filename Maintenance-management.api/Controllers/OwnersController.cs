using System.Security.Claims;
using Maintenance_management.application.DTOs.Owner;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OwnersController : ControllerBase
{
    private readonly IOwnerService _service;

    public OwnersController(IOwnerService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,DataEntry")]
    public async Task<IActionResult> Create([FromBody] CreateOwnerDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager,DataEntry")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOwnerDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/transfers")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> TransferOwnership(Guid id, [FromBody] TransferOwnershipDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var fullName = User.Identity?.Name;
        var result = await _service.TransferOwnershipAsync(id, dto, userId, fullName);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
