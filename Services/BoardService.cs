using JirafeAPI.DTOs;
using JirafeAPI.Entities;
using JirafeAPI.Repositories;

namespace JirafeAPI.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IListRepository _listRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;

    public BoardService(IBoardRepository boardRepository, IBoardMemberRepository boardMemberRepository, IListRepository listRepository, ILabelRepository labelRepository, IWorkspaceMemberRepository workspaceMemberRepository)
    {
        _boardRepository = boardRepository;
        _boardMemberRepository = boardMemberRepository;
        _listRepository = listRepository;
        _labelRepository = labelRepository;
        _workspaceMemberRepository = workspaceMemberRepository;
    }

    public async Task<BoardDto?> CreateBoardAsync(CreateBoardRequest request, int userId)
    {
        var isMember = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(request.WorkspaceId, userId);
        if (isMember == null)
            return null;

        var board = new Board
        {
            Title = request.Title,
            WorkspaceId = request.WorkspaceId,
            CreatedAt = DateTime.UtcNow
        };

        await _boardRepository.AddAsync(board);
        await _boardRepository.SaveChangesAsync();

        var boardMember = new BoardMember
        {
            BoardId = board.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        };

        await _boardMemberRepository.AddAsync(boardMember);
        await _boardMemberRepository.SaveChangesAsync();

        return new BoardDto
        {
            Id = board.Id,
            Title = board.Title,
            WorkspaceId = board.WorkspaceId,
            CreatedAt = board.CreatedAt
        };
    }

    public async Task<BoardDetailDto?> GetBoardAsync(int boardId, int userId)
    {
        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(boardId, userId);
        if (!isBoardMember)
            return null;

        var board = await _boardRepository.GetByIdAsync(boardId);
        if (board == null)
            return null;

        var lists = await _listRepository.GetByBoardIdOrderByPositionAsync(boardId);
        var labels = await _labelRepository.GetByBoardIdAsync(boardId);

        var listDtos = lists.Select(l => new ListDto
        {
            Id = l.Id,
            Title = l.Title,
            Position = l.Position,
            BoardId = l.BoardId,
            CreatedAt = l.CreatedAt,
            Cards = l.Cards.Select(c => new CardDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                DueDate = c.DueDate,
                Position = c.Position,
                ListId = c.ListId,
                CreatedAt = c.CreatedAt,
                AssignedUserIds = c.CardMembers.Select(cm => cm.UserId).ToList(),
                LabelIds = c.CardLabels.Select(cl => cl.LabelId).ToList(),
                CommentCount = c.Comments.Count
            }).ToList()
        }).ToList();

        var labelDtos = labels.Select(l => new LabelDto
        {
            Id = l.Id,
            Name = l.Name,
            ColorHex = l.ColorHex,
            BoardId = l.BoardId
        }).ToList();

        return new BoardDetailDto
        {
            Id = board.Id,
            Title = board.Title,
            WorkspaceId = board.WorkspaceId,
            CreatedAt = board.CreatedAt,
            Lists = listDtos,
            Labels = labelDtos
        };
    }

    public async Task<List<BoardDto>> GetWorkspaceBoardsAsync(int workspaceId, int userId)
    {
        var isMember = await _workspaceMemberRepository.GetByWorkspaceAndUserAsync(workspaceId, userId);
        if (isMember == null)
            return new List<BoardDto>();

        var boards = await _boardRepository.GetByWorkspaceIdAsync(workspaceId);
        return boards.Select(b => new BoardDto
        {
            Id = b.Id,
            Title = b.Title,
            WorkspaceId = b.WorkspaceId,
            CreatedAt = b.CreatedAt
        }).ToList();
    }

    public async Task<bool> UpdateBoardAsync(int boardId, UpdateBoardRequest request, int userId)
    {
        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(boardId, userId);
        if (!isBoardMember)
            return false;

        var board = await _boardRepository.GetByIdAsync(boardId);
        if (board == null)
            return false;

        board.Title = request.Title;

        await _boardRepository.UpdateAsync(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteBoardAsync(int boardId, int userId)
    {
        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(boardId, userId);
        if (!isBoardMember)
            return false;

        var board = await _boardRepository.GetByIdAsync(boardId);
        if (board == null)
            return false;

        await _boardRepository.DeleteAsync(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }
}

