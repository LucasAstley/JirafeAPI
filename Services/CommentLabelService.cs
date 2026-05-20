using JirafeAPI.DTOs;
using JirafeAPI.Entities;
using JirafeAPI.Repositories;

namespace JirafeAPI.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IListRepository _listRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;

    public CommentService(ICommentRepository commentRepository, ICardRepository cardRepository, IListRepository listRepository, IBoardMemberRepository boardMemberRepository)
    {
        _commentRepository = commentRepository;
        _cardRepository = cardRepository;
        _listRepository = listRepository;
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<CommentDto?> CreateCommentAsync(CreateCommentRequest request, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
            return null;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return null;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return null;

        var comment = new Comment
        {
            Content = request.Content,
            CardId = request.CardId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorName = comment.User?.Username,
            UserId = comment.UserId,
            CardId = comment.CardId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public async Task<List<CommentDto>> GetCardCommentsAsync(int cardId, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return new List<CommentDto>();

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return new List<CommentDto>();

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return new List<CommentDto>();

        var comments = await _commentRepository.GetByCardIdAsync(cardId);

        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            AuthorName = c.User?.Username ?? "Unknown",
            UserId = c.UserId,
            CardId = c.CardId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();
    }

    public async Task<bool> UpdateCommentAsync(int commentId, UpdateCommentRequest request, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null || comment.UserId != userId)
            return false;

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(comment);
        await _commentRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null || comment.UserId != userId)
            return false;

        await _commentRepository.DeleteAsync(comment);
        await _commentRepository.SaveChangesAsync();

        return true;
    }
}

public class LabelService : ILabelService
{
    private readonly ILabelRepository _labelRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;

    public LabelService(ILabelRepository labelRepository, IBoardMemberRepository boardMemberRepository)
    {
        _labelRepository = labelRepository;
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<LabelDto?> CreateLabelAsync(CreateLabelRequest request, int userId)
    {
        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(request.BoardId, userId);
        if (!isBoardMember)
            return null;

        var label = new Label
        {
            Name = request.Name,
            ColorHex = request.ColorHex,
            BoardId = request.BoardId
        };

        await _labelRepository.AddAsync(label);
        await _labelRepository.SaveChangesAsync();

        return new LabelDto
        {
            Id = label.Id,
            Name = label.Name,
            ColorHex = label.ColorHex,
            BoardId = label.BoardId
        };
    }

    public async Task<List<LabelDto>> GetBoardLabelsAsync(int boardId, int userId)
    {
        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(boardId, userId);
        if (!isBoardMember)
            return new List<LabelDto>();

        var labels = await _labelRepository.GetByBoardIdAsync(boardId);

        return labels.Select(l => new LabelDto
        {
            Id = l.Id,
            Name = l.Name,
            ColorHex = l.ColorHex,
            BoardId = l.BoardId
        }).ToList();
    }

    public async Task<bool> DeleteLabelAsync(int labelId, int userId)
    {
        var label = await _labelRepository.GetByIdAsync(labelId);
        if (label == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(label.BoardId, userId);
        if (!isBoardMember)
            return false;

        await _labelRepository.DeleteAsync(label);
        await _labelRepository.SaveChangesAsync();

        return true;
    }
}

