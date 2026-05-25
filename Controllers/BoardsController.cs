using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JirafeAPI.DTOs;
using JirafeAPI.Services;
using System.Security.Claims;

namespace JirafeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardsController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
    {
        var userId = GetUserId();
        var result = await _boardService.CreateBoardAsync(request, userId);
        if (result == null)
            return BadRequest("Failed to create board.");

        return CreatedAtAction(nameof(GetBoard), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBoard(int id)
    {
        var userId = GetUserId();
        var result = await _boardService.GetBoardAsync(id, userId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("workspace/{workspaceId}")]
    public async Task<IActionResult> GetWorkspaceBoards(int workspaceId)
    {
        var userId = GetUserId();
        var result = await _boardService.GetWorkspaceBoardsAsync(workspaceId, userId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoard(int id, [FromBody] UpdateBoardRequest request)
    {
        var userId = GetUserId();
        var result = await _boardService.UpdateBoardAsync(id, request, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(int id)
    {
        var userId = GetUserId();
        var result = await _boardService.DeleteBoardAsync(id, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }
}

