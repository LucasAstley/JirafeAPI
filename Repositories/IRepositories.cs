using JirafeAPI.Entities;

namespace JirafeAPI.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
}

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<List<RefreshToken>> GetByUserIdAsync(int userId);
    Task RevokeAllByUserIdAsync(int userId);
}

public interface IWorkspaceRepository : IBaseRepository<Workspace>
{
    Task<List<Workspace>> GetByUserIdAsync(int userId);
}

public interface IWorkspaceMemberRepository : IBaseRepository<WorkspaceMember>
{
    Task<List<WorkspaceMember>> GetByWorkspaceIdAsync(int workspaceId);
    Task<List<WorkspaceMember>> GetByUserIdAsync(int userId);
    Task<WorkspaceMember?> GetByWorkspaceAndUserAsync(int workspaceId, int userId);
}

public interface IBoardRepository : IBaseRepository<Board>
{
    Task<List<Board>> GetByWorkspaceIdAsync(int workspaceId);
}

public interface IBoardMemberRepository : IBaseRepository<BoardMember>
{
    Task<List<BoardMember>> GetByBoardIdAsync(int boardId);
    Task<bool> IsBoardMemberAsync(int boardId, int userId);
}

public interface IListRepository : IBaseRepository<List>
{
    Task<List<Entities.List>> GetByBoardIdOrderByPositionAsync(int boardId);
}

public interface ICardRepository : IBaseRepository<Card>
{
    Task<List<Card>> GetByListIdOrderByPositionAsync(int listId);
    Task<List<Card>> GetByBoardIdAsync(int boardId);
}

public interface ICardMemberRepository : IBaseRepository<CardMember>
{
    Task<List<CardMember>> GetByCardIdAsync(int cardId);
    Task<List<int>> GetAssignedUserIdsByCardIdAsync(int cardId);
}

public interface ILabelRepository : IBaseRepository<Label>
{
    Task<List<Label>> GetByBoardIdAsync(int boardId);
}

public interface ICardLabelRepository : IBaseRepository<CardLabel>
{
    Task<List<CardLabel>> GetByCardIdAsync(int cardId);
    Task<List<int>> GetLabelIdsByCardIdAsync(int cardId);
}

public interface ICommentRepository : IBaseRepository<Comment>
{
    Task<List<Comment>> GetByCardIdAsync(int cardId);
    Task<int> GetCommentCountByCardIdAsync(int cardId);
}

