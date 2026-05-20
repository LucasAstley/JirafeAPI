using JirafeAPI.DTOs;
using JirafeAPI.Entities;
using JirafeAPI.Repositories;

namespace JirafeAPI.Services;

public class ListService : IListService
{
    private readonly IListRepository _listRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;

    public ListService(IListRepository listRepository, IBoardMemberRepository boardMemberRepository)
    {
        _listRepository = listRepository;
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<ListDto?> CreateListAsync(CreateListRequest request, int userId)
    {
        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(request.BoardId, userId);
        if (!isBoardMember)
            return null;

        var list = new Entities.List
        {
            Title = request.Title,
            Position = request.Position,
            BoardId = request.BoardId,
            CreatedAt = DateTime.UtcNow
        };

        await _listRepository.AddAsync(list);
        await _listRepository.SaveChangesAsync();

        return new ListDto
        {
            Id = list.Id,
            Title = list.Title,
            Position = list.Position,
            BoardId = list.BoardId,
            CreatedAt = list.CreatedAt,
            Cards = new List<CardDto>()
        };
    }

    public async Task<bool> UpdateListAsync(int listId, UpdateListRequest request, int userId)
    {
        var list = await _listRepository.GetByIdAsync(listId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        if (!string.IsNullOrEmpty(request.Title))
            list.Title = request.Title;

        if (request.Position.HasValue)
            list.Position = request.Position.Value;

        await _listRepository.UpdateAsync(list);
        await _listRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteListAsync(int listId, int userId)
    {
        var list = await _listRepository.GetByIdAsync(listId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        await _listRepository.DeleteAsync(list);
        await _listRepository.SaveChangesAsync();

        return true;
    }
}

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly ICardMemberRepository _cardMemberRepository;
    private readonly ICardLabelRepository _cardLabelRepository;
    private readonly IListRepository _listRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;

    public CardService(ICardRepository cardRepository, ICardMemberRepository cardMemberRepository, ICardLabelRepository cardLabelRepository, IListRepository listRepository, IBoardMemberRepository boardMemberRepository)
    {
        _cardRepository = cardRepository;
        _cardMemberRepository = cardMemberRepository;
        _cardLabelRepository = cardLabelRepository;
        _listRepository = listRepository;
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<CardDto?> CreateCardAsync(CreateCardRequest request, int userId)
    {
        var list = await _listRepository.GetByIdAsync(request.ListId);
        if (list == null)
            return null;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return null;

        var card = new Card
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Position = request.Position,
            ListId = request.ListId,
            CreatedAt = DateTime.UtcNow
        };

        await _cardRepository.AddAsync(card);
        await _cardRepository.SaveChangesAsync();

        return new CardDto
        {
            Id = card.Id,
            Title = card.Title,
            Description = card.Description,
            DueDate = card.DueDate,
            Position = card.Position,
            ListId = card.ListId,
            CreatedAt = card.CreatedAt,
            AssignedUserIds = new List<int>(),
            LabelIds = new List<int>(),
            CommentCount = 0
        };
    }

    public async Task<CardDto?> GetCardAsync(int cardId, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return null;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return null;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return null;

        var assignedUserIds = await _cardMemberRepository.GetAssignedUserIdsByCardIdAsync(cardId);
        var labelIds = await _cardLabelRepository.GetLabelIdsByCardIdAsync(cardId);

        return new CardDto
        {
            Id = card.Id,
            Title = card.Title,
            Description = card.Description,
            DueDate = card.DueDate,
            Position = card.Position,
            ListId = card.ListId,
            CreatedAt = card.CreatedAt,
            AssignedUserIds = assignedUserIds,
            LabelIds = labelIds,
            CommentCount = card.Comments.Count
        };
    }

    public async Task<bool> UpdateCardAsync(int cardId, UpdateCardRequest request, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        if (!string.IsNullOrEmpty(request.Title))
            card.Title = request.Title;

        if (request.Description != null)
            card.Description = request.Description;

        if (request.DueDate.HasValue)
            card.DueDate = request.DueDate;

        await _cardRepository.UpdateAsync(card);
        await _cardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateCardPositionAsync(int cardId, UpdateCardPositionRequest request, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        card.ListId = request.ListId;
        card.Position = request.Position;

        await _cardRepository.UpdateAsync(card);
        await _cardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCardAsync(int cardId, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        await _cardRepository.DeleteAsync(card);
        await _cardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AssignUserToCardAsync(int cardId, int userId, int assignedUserId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        var cardMember = new CardMember
        {
            CardId = cardId,
            UserId = assignedUserId
        };

        await _cardMemberRepository.AddAsync(cardMember);
        await _cardMemberRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveUserFromCardAsync(int cardId, int userId, int assignedUserId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        var cardMember = card.CardMembers.FirstOrDefault(cm => cm.UserId == assignedUserId);
        if (cardMember == null)
            return false;

        await _cardMemberRepository.DeleteAsync(cardMember);
        await _cardMemberRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddLabelToCardAsync(int cardId, int userId, int labelId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        var cardLabel = new CardLabel
        {
            CardId = cardId,
            LabelId = labelId
        };

        await _cardLabelRepository.AddAsync(cardLabel);
        await _cardLabelRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveLabelFromCardAsync(int cardId, int userId, int labelId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        var list = await _listRepository.GetByIdAsync(card.ListId);
        if (list == null)
            return false;

        var isBoardMember = await _boardMemberRepository.IsBoardMemberAsync(list.BoardId, userId);
        if (!isBoardMember)
            return false;

        var cardLabel = card.CardLabels.FirstOrDefault(cl => cl.LabelId == labelId);
        if (cardLabel == null)
            return false;

        await _cardLabelRepository.DeleteAsync(cardLabel);
        await _cardLabelRepository.SaveChangesAsync();

        return true;
    }
}

