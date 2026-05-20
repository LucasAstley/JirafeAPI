using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JirafeAPI.DTOs;
using JirafeAPI.Services;
using System.Security.Claims;

namespace JirafeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
    {
        var userId = GetUserId();
        var result = await _commentService.CreateCommentAsync(request, userId);
        if (result == null)
            return BadRequest("Failed to create comment.");

        return Created($"/api/comments/{result.Id}", result);
    }

    [HttpGet("card/{cardId}")]
    public async Task<IActionResult> GetCardComments(int cardId)
    {
        var userId = GetUserId();
        var result = await _commentService.GetCardCommentsAsync(cardId, userId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest request)
    {
        var userId = GetUserId();
        var result = await _commentService.UpdateCommentAsync(id, request, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var userId = GetUserId();
        var result = await _commentService.DeleteCommentAsync(id, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabelsController : ControllerBase
{
    private readonly ILabelService _labelService;

    public LabelsController(ILabelService labelService)
    {
        _labelService = labelService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    [HttpPost]
    public async Task<IActionResult> CreateLabel([FromBody] CreateLabelRequest request)
    {
        var userId = GetUserId();
        var result = await _labelService.CreateLabelAsync(request, userId);
        if (result == null)
            return BadRequest("Failed to create label.");

        return Created($"/api/labels/{result.Id}", result);
    }

    [HttpGet("board/{boardId}")]
    public async Task<IActionResult> GetBoardLabels(int boardId)
    {
        var userId = GetUserId();
        var result = await _labelService.GetBoardLabelsAsync(boardId, userId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLabel(int id)
    {
        var userId = GetUserId();
        var result = await _labelService.DeleteLabelAsync(id, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }
}

