using JirafeAPI.DTOs;
using JirafeAPI.Entities;
using JirafeAPI.Repositories;

namespace JirafeAPI.Services;

public class WorkspaceService : IWorkspaceService
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;

    public WorkspaceService(
        IWorkspaceRepository workspaceRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IUserRepository userRepository,
        IBoardRepository boardRepository,
        IBoardMemberRepository boardMemberRepository)
    {
        _workspaceRepository = workspaceRepository;
        _workspaceMemberRepository = workspaceMemberRepository;
        _userRepository = userRepository;
        _boardRepository = boardRepository;
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<WorkspaceDto?> CreateWorkspaceAsync(CreateWorkspaceRequest request, int userId)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _workspaceRepository.AddAsync(workspace);
        await _workspaceRepository.SaveChangesAsync();

        var member = new WorkspaceMember
        {
            WorkspaceId = workspace.Id,
            UserId = userId,
            Role = "Owner",
            JoinedAt = DateTime.UtcNow
        };

        await _workspaceMemberRepository.AddAsync(member);
        await _workspaceMemberRepository.SaveChangesAsync();

        return new WorkspaceDto
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Description = workspace.Description,
            CreatedAt = workspace.CreatedAt
        };
    }

    public async Task<WorkspaceDto?> GetWorkspaceAsync(int workspaceId, int userId)
    {
        var isMember = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(workspaceId, userId);
        if (isMember == null)
            return null;

        var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
        if (workspace == null)
            return null;

        return new WorkspaceDto
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Description = workspace.Description,
            CreatedAt = workspace.CreatedAt
        };
    }

    public async Task<List<WorkspaceDto>> GetUserWorkspacesAsync(int userId)
    {
        var workspaces = await _workspaceRepository.GetByUserIdAsync(userId);
        return workspaces.Select(w => new WorkspaceDto
        {
            Id = w.Id,
            Name = w.Name,
            Description = w.Description,
            CreatedAt = w.CreatedAt
        }).ToList();
    }

    public async Task<bool> UpdateWorkspaceAsync(int workspaceId, UpdateWorkspaceRequest request, int userId)
    {
        var member = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(workspaceId, userId);
        if (member == null || member.Role != "Owner")
            return false;

        var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
        if (workspace == null)
            return false;

        workspace.Name = request.Name;
        workspace.Description = request.Description;

        await _workspaceRepository.UpdateAsync(workspace);
        await _workspaceRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteWorkspaceAsync(int workspaceId, int userId)
    {
        var member = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(workspaceId, userId);
        if (member == null || member.Role != "Owner")
            return false;

        var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
        if (workspace == null)
            return false;

        await _workspaceRepository.DeleteAsync(workspace);
        await _workspaceRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> InviteMemberAsync(int workspaceId, InviteWorkspaceMemberRequest request, int userId)
    {
        var requester = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(workspaceId, userId);
        if (requester == null || (requester.Role != "Owner" && requester.Role != "Admin"))
            return false;

        var targetUser = await _userRepository.GetByEmailAsync(request.Email);
        if (targetUser == null)
            return false;

        var existingMember = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(workspaceId, targetUser.Id);
        if (existingMember != null)
            return false;

        var member = new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = targetUser.Id,
            Role = request.Role,
            JoinedAt = DateTime.UtcNow
        };

        await _workspaceMemberRepository.AddAsync(member);
        await _workspaceMemberRepository.SaveChangesAsync();

        var boards = await _boardRepository.GetByWorkspaceIdAsync(workspaceId);
        foreach (var board in boards)
        {
            var alreadyBoardMember = await _boardMemberRepository.IsBoardMemberAsync(board.Id, targetUser.Id);
            if (alreadyBoardMember) continue;

            await _boardMemberRepository.AddAsync(new BoardMember
            {
                BoardId = board.Id,
                UserId = targetUser.Id,
                JoinedAt = DateTime.UtcNow
            });
        }
        await _boardMemberRepository.SaveChangesAsync();

        return true;
    }

    public async Task<List<WorkspaceMemberDto>> GetWorkspaceMembersAsync(int workspaceId, int userId)
    {
        var isMember = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(workspaceId, userId);
        if (isMember == null)
            return new List<WorkspaceMemberDto>();

        var members = await _workspaceMemberRepository.GetByWorkspaceIdAsync(workspaceId);
        return members.Select(m => new WorkspaceMemberDto
        {
            Id = m.Id,
            WorkspaceId = m.WorkspaceId,
            UserId = m.UserId,
            UserName = m.User.Username,
            UserEmail = m.User.Email,
            Role = m.Role,
            JoinedAt = m.JoinedAt
        }).ToList();
    }
}
