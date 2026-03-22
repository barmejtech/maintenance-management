using Maintenance_management.application.DTOs.Group;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _service;

    public GroupsController(IGroupService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateGroupDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{groupId:guid}/members")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddMember(Guid groupId, [FromBody] AddMemberDto dto)
    {
        var result = await _service.AddMemberAsync(groupId, dto.TechnicianId);
        return result ? Ok() : BadRequest();
    }

    [HttpDelete("{groupId:guid}/members/{technicianId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RemoveMember(Guid groupId, Guid technicianId)
    {
        var result = await _service.RemoveMemberAsync(groupId, technicianId);
        return result ? NoContent() : NotFound();
    }
}
