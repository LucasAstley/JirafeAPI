using Microsoft.AspNetCore.SignalR;

namespace JirafeAPI.Hubs;

public class BoardHub : Hub
{
    public async Task JoinBoard(int boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"board-{boardId}");
    }

    public async Task LeaveBoard(int boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board-{boardId}");
    }

    public async Task CardMoved(int boardId, int cardId, int newListId, int newPosition)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CardMoved", cardId, newListId, newPosition);
    }

    public async Task CardUpdated(int boardId, int cardId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CardUpdated", cardId);
    }

    public async Task CardDeleted(int boardId, int cardId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CardDeleted", cardId);
    }

    public async Task ListPositionChanged(int boardId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("ListPositionChanged");
    }

    public async Task CommentAdded(int boardId, int cardId, int commentId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CommentAdded", cardId, commentId);
    }

    public async Task CommentDeleted(int boardId, int cardId, int commentId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CommentDeleted", cardId, commentId);
    }
}

