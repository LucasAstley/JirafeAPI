using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JirafeAPI.DTOs;
using JirafeAPI.Services;
using System.Security.Claims;

namespace JirafeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public WorkspacesController(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest request)
    {
        var userId = GetUserId();
        var result = await _workspaceService.CreateWorkspaceAsync(request, userId);
        if (result == null)
            return BadRequest("Failed to create workspace.");

        return CreatedAtAction(nameof(GetWorkspace), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkspace(int id)
    {
        var userId = GetUserId();
        var result = await _workspaceService.GetWorkspaceAsync(id, userId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWorkspaces()
    {
        var userId = GetUserId();
        var result = await _workspaceService.GetUserWorkspacesAsync(userId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkspace(int id, [FromBody] UpdateWorkspaceRequest request)
    {
        var userId = GetUserId();
        var result = await _workspaceService.UpdateWorkspaceAsync(id, request, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkspace(int id)
    {
        var userId = GetUserId();
        var result = await _workspaceService.DeleteWorkspaceAsync(id, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpPost("{id}/members")]
    public async Task<IActionResult> InviteMember(int id, [FromBody] InviteWorkspaceMemberRequest request)
    {
        var userId = GetUserId();
        var result = await _workspaceService.InviteMemberAsync(id, request, userId);
        if (!result)
            return BadRequest("Failed to invite member.");

        return Ok("Member invited successfully.");
    }

    [HttpGet("{id}/members")]
    public async Task<IActionResult> GetMembers(int id)
    {
        var userId = GetUserId();
        var result = await _workspaceService.GetWorkspaceMembersAsync(id, userId);
        if (!result.Any())
            return NotFound();

        return Ok(result);
    }
}

