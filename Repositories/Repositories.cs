using Microsoft.EntityFrameworkCore;
using JirafeAPI.Data;
using JirafeAPI.Entities;

namespace JirafeAPI.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }
}

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
    }

    public async Task<List<RefreshToken>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(rt => rt.UserId == userId && !rt.IsRevoked).ToListAsync();
    }

    public async Task RevokeAllByUserIdAsync(int userId)
    {
        var tokens = await _dbSet.Where(rt => rt.UserId == userId && !rt.IsRevoked).ToListAsync();
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
    }
}

public class WorkspaceRepository : BaseRepository<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<Workspace>> GetByUserIdAsync(int userId)
    {
        return await _context.WorkspaceMembers
            .Where(wm => wm.UserId == userId)
            .Include(wm => wm.Workspace)
            .Select(wm => wm.Workspace)
            .ToListAsync();
    }
}

public class WorkspaceMemberRepository : BaseRepository<WorkspaceMember>, IWorkspaceMemberRepository
{
    public WorkspaceMemberRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<WorkspaceMember>> GetByWorkspaceIdAsync(int workspaceId)
    {
        return await _dbSet
            .Where(wm => wm.WorkspaceId == workspaceId)
            .Include(wm => wm.User)
            .ToListAsync();
    }

    public async Task<List<WorkspaceMember>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(wm => wm.UserId == userId)
            .Include(wm => wm.Workspace)
            .ToListAsync();
    }

    public async Task<WorkspaceMember?> GetByWorkspaceAndUserAsync(int workspaceId, int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId);
    }
}

public class BoardRepository : BaseRepository<Board>, IBoardRepository
{
    public BoardRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<Board>> GetByWorkspaceIdAsync(int workspaceId)
    {
        return await _dbSet.Where(b => b.WorkspaceId == workspaceId).ToListAsync();
    }
}

public class BoardMemberRepository : BaseRepository<BoardMember>, IBoardMemberRepository
{
    public BoardMemberRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<BoardMember>> GetByBoardIdAsync(int boardId)
    {
        return await _dbSet
            .Where(bm => bm.BoardId == boardId)
            .Include(bm => bm.User)
            .ToListAsync();
    }

    public async Task<bool> IsBoardMemberAsync(int boardId, int userId)
    {
        return await _dbSet.AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId);
    }
}

public class ListRepository : BaseRepository<Entities.List>, IListRepository
{
    public ListRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<Entities.List>> GetByBoardIdOrderByPositionAsync(int boardId)
    {
        return await _dbSet
            .Where(l => l.BoardId == boardId)
            .OrderBy(l => l.Position)
            .Include(l => l.Cards.OrderBy(c => c.Position))
                .ThenInclude(c => c.CardMembers)
            .Include(l => l.Cards.OrderBy(c => c.Position))
                .ThenInclude(c => c.CardLabels)
            .Include(l => l.Cards.OrderBy(c => c.Position))
                .ThenInclude(c => c.Comments)
            .ToListAsync();
    }
}

public class CardRepository : BaseRepository<Card>, ICardRepository
{
    public CardRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<Card>> GetByListIdOrderByPositionAsync(int listId)
    {
        return await _dbSet
            .Where(c => c.ListId == listId)
            .OrderBy(c => c.Position)
            .Include(c => c.CardMembers)
            .Include(c => c.CardLabels)
            .ToListAsync();
    }

    public async Task<List<Card>> GetByBoardIdAsync(int boardId)
    {
        return await _context.Lists
            .Where(l => l.BoardId == boardId)
            .SelectMany(l => l.Cards)
            .ToListAsync();
    }
}

public class CardMemberRepository : BaseRepository<CardMember>, ICardMemberRepository
{
    public CardMemberRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<CardMember>> GetByCardIdAsync(int cardId)
    {
        return await _dbSet
            .Where(cm => cm.CardId == cardId)
            .Include(cm => cm.User)
            .ToListAsync();
    }

    public async Task<List<int>> GetAssignedUserIdsByCardIdAsync(int cardId)
    {
        return await _dbSet
            .Where(cm => cm.CardId == cardId)
            .Select(cm => cm.UserId)
            .ToListAsync();
    }
}

public class LabelRepository : BaseRepository<Label>, ILabelRepository
{
    public LabelRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<Label>> GetByBoardIdAsync(int boardId)
    {
        return await _dbSet.Where(l => l.BoardId == boardId).ToListAsync();
    }
}

public class CardLabelRepository : BaseRepository<CardLabel>, ICardLabelRepository
{
    public CardLabelRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<CardLabel>> GetByCardIdAsync(int cardId)
    {
        return await _dbSet
            .Where(cl => cl.CardId == cardId)
            .Include(cl => cl.Label)
            .ToListAsync();
    }

    public async Task<List<int>> GetLabelIdsByCardIdAsync(int cardId)
    {
        return await _dbSet
            .Where(cl => cl.CardId == cardId)
            .Select(cl => cl.LabelId)
            .ToListAsync();
    }
}

public class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(TaskBoardDbContext context) : base(context) { }

    public async Task<List<Comment>> GetByCardIdAsync(int cardId)
    {
        return await _dbSet
            .Where(c => c.CardId == cardId)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetCommentCountByCardIdAsync(int cardId)
    {
        return await _dbSet.CountAsync(c => c.CardId == cardId);
    }
}
