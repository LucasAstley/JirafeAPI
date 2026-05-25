namespace JirafeAPI.Entities;

public class Card
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ListId { get; set; }

    public virtual List List { get; set; } = null!;
    public virtual ICollection<CardMember> CardMembers { get; set; } = new List<CardMember>();
    public virtual ICollection<CardLabel> CardLabels { get; set; } = new List<CardLabel>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

