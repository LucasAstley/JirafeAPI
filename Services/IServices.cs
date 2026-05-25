using JirafeAPI.DTOs;

namespace JirafeAPI.Services;

public interface IAuthService
{
    Task<UserLoginResponse?> RegisterAsync(UserRegisterRequest request);
    Task<UserLoginResponse?> LoginAsync(UserLoginRequest request);
    Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(int userId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IWorkspaceService
{
    Task<WorkspaceDto?> CreateWorkspaceAsync(CreateWorkspaceRequest request, int userId);
    Task<WorkspaceDto?> GetWorkspaceAsync(int workspaceId, int userId);
    Task<List<WorkspaceDto>> GetUserWorkspacesAsync(int userId);
    Task<bool> UpdateWorkspaceAsync(int workspaceId, UpdateWorkspaceRequest request, int userId);
    Task<bool> DeleteWorkspaceAsync(int workspaceId, int userId);
    Task<bool> InviteMemberAsync(int workspaceId, InviteWorkspaceMemberRequest request, int userId);
    Task<List<WorkspaceMemberDto>> GetWorkspaceMembersAsync(int workspaceId, int userId);
}

public interface IBoardService
{
    Task<BoardDto?> CreateBoardAsync(CreateBoardRequest request, int userId);
    Task<BoardDetailDto?> GetBoardAsync(int boardId, int userId);
    Task<List<BoardDto>> GetWorkspaceBoardsAsync(int workspaceId, int userId);
    Task<bool> UpdateBoardAsync(int boardId, UpdateBoardRequest request, int userId);
    Task<bool> DeleteBoardAsync(int boardId, int userId);
}

public interface IListService
{
    Task<ListDto?> CreateListAsync(CreateListRequest request, int userId);
    Task<bool> UpdateListAsync(int listId, UpdateListRequest request, int userId);
    Task<bool> DeleteListAsync(int listId, int userId);
}

public interface ICardService
{
    Task<CardDto?> CreateCardAsync(CreateCardRequest request, int userId);
    Task<CardDto?> GetCardAsync(int cardId, int userId);
    Task<bool> UpdateCardAsync(int cardId, UpdateCardRequest request, int userId);
    Task<bool> UpdateCardPositionAsync(int cardId, UpdateCardPositionRequest request, int userId);
    Task<bool> DeleteCardAsync(int cardId, int userId);
    Task<bool> AssignUserToCardAsync(int cardId, int userId, int assignedUserId);
    Task<bool> RemoveUserFromCardAsync(int cardId, int userId, int assignedUserId);
    Task<bool> AddLabelToCardAsync(int cardId, int userId, int labelId);
    Task<bool> RemoveLabelFromCardAsync(int cardId, int userId, int labelId);
}

public interface ICommentService
{
    Task<CommentDto?> CreateCommentAsync(CreateCommentRequest request, int userId);
    Task<List<CommentDto>> GetCardCommentsAsync(int cardId, int userId);
    Task<bool> UpdateCommentAsync(int commentId, UpdateCommentRequest request, int userId);
    Task<bool> DeleteCommentAsync(int commentId, int userId);
}

public interface ILabelService
{
    Task<LabelDto?> CreateLabelAsync(CreateLabelRequest request, int userId);
    Task<List<LabelDto>> GetBoardLabelsAsync(int boardId, int userId);
    Task<bool> DeleteLabelAsync(int labelId, int userId);
}

