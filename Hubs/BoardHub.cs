using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JirafeAPI.Hubs;

[Authorize]
public class BoardHub : Hub
{
    private static readonly ConcurrentDictionary<string, HashSet<int>> ConnectionBoards = new();
    private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, PresenceUser>> BoardConnections = new();

    public static string BoardGroup(int boardId) => $"board-{boardId}";
    public static string WorkspaceGroup(int workspaceId) => $"workspace-{workspaceId}";

    public async Task JoinBoard(int boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, BoardGroup(boardId));

        var joinedBoards = ConnectionBoards.GetOrAdd(Context.ConnectionId, _ => new HashSet<int>());
        lock (joinedBoards)
        {
            joinedBoards.Add(boardId);
        }

        var boardUsers = BoardConnections.GetOrAdd(boardId, _ => new ConcurrentDictionary<string, PresenceUser>());
        boardUsers[Context.ConnectionId] = BuildPresenceUser();

        await BroadcastPresence(boardId);
    }

    public async Task LeaveBoard(int boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, BoardGroup(boardId));

        if (ConnectionBoards.TryGetValue(Context.ConnectionId, out var joinedBoards))
        {
            lock (joinedBoards)
            {
                joinedBoards.Remove(boardId);
            }
        }

        if (BoardConnections.TryGetValue(boardId, out var boardUsers))
        {
            boardUsers.TryRemove(Context.ConnectionId, out _);
            if (boardUsers.IsEmpty)
            {
                BoardConnections.TryRemove(boardId, out _);
            }
        }

        await BroadcastPresence(boardId);
    }

    public async Task JoinWorkspace(int workspaceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, WorkspaceGroup(workspaceId));
    }

    public async Task LeaveWorkspace(int workspaceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, WorkspaceGroup(workspaceId));
    }

    public async Task CardEditing(int boardId, int cardId)
    {
        await Clients.OthersInGroup(BoardGroup(boardId))
            .SendAsync("CardEditing", new
            {
                boardId,
                cardId,
                userId = GetUserId(),
                username = GetUsername()
            });
    }

    public async Task CardEditStopped(int boardId, int cardId)
    {
        await Clients.OthersInGroup(BoardGroup(boardId))
            .SendAsync("CardEditStopped", new
            {
                boardId,
                cardId,
                userId = GetUserId()
            });
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (ConnectionBoards.TryRemove(Context.ConnectionId, out var joinedBoards))
        {
            List<int> boards;
            lock (joinedBoards)
            {
                boards = joinedBoards.ToList();
            }

            foreach (var boardId in boards)
            {
                if (BoardConnections.TryGetValue(boardId, out var boardUsers))
                {
                    boardUsers.TryRemove(Context.ConnectionId, out _);
                    if (boardUsers.IsEmpty)
                    {
                        BoardConnections.TryRemove(boardId, out _);
                    }
                }

                await BroadcastPresence(boardId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task BroadcastPresence(int boardId)
    {
        var users = BoardConnections.TryGetValue(boardId, out var boardUsers)
            ? boardUsers.Values.DistinctBy(u => u.UserId).ToList()
            : new List<PresenceUser>();

        await Clients.Group(BoardGroup(boardId))
            .SendAsync("BoardPresenceUpdated", new { boardId, users });
    }

    private PresenceUser BuildPresenceUser()
    {
        var userId = GetUserId();
        return new PresenceUser
        {
            UserId = userId,
            Username = GetUsername(),
            ConnectedAtUtc = DateTime.UtcNow
        };
    }

    private int GetUserId()
    {
        var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var userId) ? userId : 0;
    }

    private string GetUsername()
    {
        return Context.User?.FindFirst(ClaimTypes.Name)?.Value
               ?? Context.User?.FindFirst("unique_name")?.Value
               ?? "Unknown";
    }
}

public class PresenceUser
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime ConnectedAtUtc { get; set; }
}

file static class LinqDistinctByCompat
{
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var seen = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seen.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}
