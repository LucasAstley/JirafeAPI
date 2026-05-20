namespace JirafeAPI.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CardId { get; set; }
    public int? UserId { get; set; }

    public virtual Card Card { get; set; } = null!;
    public virtual User? User { get; set; }
}

