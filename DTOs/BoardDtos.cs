namespace JirafeAPI.DTOs;

public class BoardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int WorkspaceId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBoardRequest
{
    public string Title { get; set; } = string.Empty;
    public int WorkspaceId { get; set; }
}

public class UpdateBoardRequest
{
    public string Title { get; set; } = string.Empty;
}

public class BoardDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int WorkspaceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ListDto> Lists { get; set; } = new();
    public List<LabelDto> Labels { get; set; } = new();
}

public class ListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public int BoardId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CardDto> Cards { get; set; } = new();
}

public class CreateListRequest
{
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public int BoardId { get; set; }
}

public class UpdateListRequest
{
    public string? Title { get; set; }
    public int? Position { get; set; }
}

public class CardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Position { get; set; }
    public int ListId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<int> AssignedUserIds { get; set; } = new();
    public List<int> LabelIds { get; set; } = new();
    public int CommentCount { get; set; }
}

public class CreateCardRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Position { get; set; }
    public int ListId { get; set; }
}

public class UpdateCardRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdateCardPositionRequest
{
    public int ListId { get; set; }
    public int Position { get; set; }
}

public class LabelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public int BoardId { get; set; }
}

public class CreateLabelRequest
{
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public int BoardId { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public int? UserId { get; set; }
    public int CardId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public int CardId { get; set; }
}

public class UpdateCommentRequest
{
    public string Content { get; set; } = string.Empty;
}

