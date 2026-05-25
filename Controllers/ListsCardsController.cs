using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JirafeAPI.DTOs;
using JirafeAPI.Services;
using System.Security.Claims;

namespace JirafeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ListsController : ControllerBase
{
    private readonly IListService _listService;

    public ListsController(IListService listService)
    {
        _listService = listService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    [HttpPost]
    public async Task<IActionResult> CreateList([FromBody] CreateListRequest request)
    {
        var userId = GetUserId();
        var result = await _listService.CreateListAsync(request, userId);
        if (result == null)
            return BadRequest("Failed to create list.");

        return Created($"/api/lists/{result.Id}", result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateList(int id, [FromBody] UpdateListRequest request)
    {
        var userId = GetUserId();
        var result = await _listService.UpdateListAsync(id, request, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteList(int id)
    {
        var userId = GetUserId();
        var result = await _listService.DeleteListAsync(id, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] CreateCardRequest request)
    {
        var userId = GetUserId();
        var result = await _cardService.CreateCardAsync(request, userId);
        if (result == null)
            return BadRequest("Failed to create card.");

        return CreatedAtAction(nameof(GetCard), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCard(int id)
    {
        var userId = GetUserId();
        var result = await _cardService.GetCardAsync(id, userId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCard(int id, [FromBody] UpdateCardRequest request)
    {
        var userId = GetUserId();
        var result = await _cardService.UpdateCardAsync(id, request, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpPut("{id}/position")]
    public async Task<IActionResult> UpdateCardPosition(int id, [FromBody] UpdateCardPositionRequest request)
    {
        var userId = GetUserId();
        var result = await _cardService.UpdateCardPositionAsync(id, request, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCard(int id)
    {
        var userId = GetUserId();
        var result = await _cardService.DeleteCardAsync(id, userId);
        if (!result)
            return Forbid();

        return NoContent();
    }

    [HttpPost("{id}/assign/{assignedUserId}")]
    public async Task<IActionResult> AssignUser(int id, int assignedUserId)
    {
        var userId = GetUserId();
        var result = await _cardService.AssignUserToCardAsync(id, userId, assignedUserId);
        if (!result)
            return BadRequest("Failed to assign user.");

        return Ok("User assigned successfully.");
    }

    [HttpDelete("{id}/assign/{assignedUserId}")]
    public async Task<IActionResult> RemoveAssignment(int id, int assignedUserId)
    {
        var userId = GetUserId();
        var result = await _cardService.RemoveUserFromCardAsync(id, userId, assignedUserId);
        if (!result)
            return BadRequest("Failed to remove assignment.");

        return Ok("User removed successfully.");
    }

    [HttpPost("{id}/labels/{labelId}")]
    public async Task<IActionResult> AddLabel(int id, int labelId)
    {
        var userId = GetUserId();
        var result = await _cardService.AddLabelToCardAsync(id, userId, labelId);
        if (!result)
            return BadRequest("Failed to add label.");

        return Ok("Label added successfully.");
    }

    [HttpDelete("{id}/labels/{labelId}")]
    public async Task<IActionResult> RemoveLabel(int id, int labelId)
    {
        var userId = GetUserId();
        var result = await _cardService.RemoveLabelFromCardAsync(id, userId, labelId);
        if (!result)
            return BadRequest("Failed to remove label.");

        return Ok("Label removed successfully.");
    }
}

