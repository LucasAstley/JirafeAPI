using JirafeAPI.DTOs;
using JirafeAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace JirafeAPI.Services;

public interface IBoardRealtimeNotifier
{
    Task BoardCreatedAsync(int workspaceId, BoardDto board);
    Task BoardUpdatedAsync(int boardId, string title);
    Task BoardDeletedAsync(int workspaceId, int boardId);
    Task ListCreatedAsync(int boardId, ListDto list);
    Task ListUpdatedAsync(int boardId, int listId);
    Task ListDeletedAsync(int boardId, int listId);
    Task CardCreatedAsync(int boardId, CardDto card);
    Task CardUpdatedAsync(int boardId, int cardId);
    Task CardMovedAsync(int boardId, int cardId, int listId, int position);
    Task CardDeletedAsync(int boardId, int cardId);
    Task CardMemberAssignedAsync(int boardId, int cardId, int userId);
    Task CardMemberRemovedAsync(int boardId, int cardId, int userId);
    Task LabelCreatedAsync(int boardId, LabelDto label);
    Task LabelDeletedAsync(int boardId, int labelId);
    Task CardLabelAddedAsync(int boardId, int cardId, int labelId);
    Task CardLabelRemovedAsync(int boardId, int cardId, int labelId);
    Task CommentCreatedAsync(int boardId, CommentDto comment);
    Task CommentUpdatedAsync(int boardId, int commentId);
    Task CommentDeletedAsync(int boardId, int cardId, int commentId);
}

public class BoardRealtimeNotifier : IBoardRealtimeNotifier
{
    private readonly IHubContext<BoardHub> _hubContext;

    public BoardRealtimeNotifier(IHubContext<BoardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task BoardCreatedAsync(int workspaceId, BoardDto board) =>
        _hubContext.Clients.Group(BoardHub.WorkspaceGroup(workspaceId)).SendAsync("BoardCreated", board);

    public Task BoardUpdatedAsync(int boardId, string title) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("BoardUpdated", new { boardId, title });

    public Task BoardDeletedAsync(int workspaceId, int boardId)
    {
        var payload = new { boardId };
        return Task.WhenAll(
            _hubContext.Clients.Group(BoardHub.WorkspaceGroup(workspaceId)).SendAsync("BoardDeleted", payload),
            _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("BoardDeleted", payload)
        );
    }

    public Task ListCreatedAsync(int boardId, ListDto list) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("ListCreated", list);

    public Task ListUpdatedAsync(int boardId, int listId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("ListUpdated", new { listId });

    public Task ListDeletedAsync(int boardId, int listId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("ListDeleted", new { listId });

    public Task CardCreatedAsync(int boardId, CardDto card) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("CardCreated", card);

    public Task CardUpdatedAsync(int boardId, int cardId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("CardUpdated", new { cardId });

    public Task CardMovedAsync(int boardId, int cardId, int listId, int position) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId))
            .SendAsync("CardMoved", new { cardId, listId, position });

    public Task CardDeletedAsync(int boardId, int cardId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("CardDeleted", new { cardId });

    public Task CardMemberAssignedAsync(int boardId, int cardId, int userId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId))
            .SendAsync("CardMemberAssigned", new { cardId, userId });

    public Task CardMemberRemovedAsync(int boardId, int cardId, int userId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId))
            .SendAsync("CardMemberRemoved", new { cardId, userId });

    public Task LabelCreatedAsync(int boardId, LabelDto label) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("LabelCreated", label);

    public Task LabelDeletedAsync(int boardId, int labelId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("LabelDeleted", new { labelId });

    public Task CardLabelAddedAsync(int boardId, int cardId, int labelId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId))
            .SendAsync("CardLabelAdded", new { cardId, labelId });

    public Task CardLabelRemovedAsync(int boardId, int cardId, int labelId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId))
            .SendAsync("CardLabelRemoved", new { cardId, labelId });

    public Task CommentCreatedAsync(int boardId, CommentDto comment) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("CommentCreated", comment);

    public Task CommentUpdatedAsync(int boardId, int commentId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId)).SendAsync("CommentUpdated", new { commentId });

    public Task CommentDeletedAsync(int boardId, int cardId, int commentId) =>
        _hubContext.Clients.Group(BoardHub.BoardGroup(boardId))
            .SendAsync("CommentDeleted", new { cardId, commentId });
}
