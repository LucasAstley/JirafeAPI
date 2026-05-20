namespace JirafeAPI.Entities;

public class List
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BoardId { get; set; }

    public virtual Board Board { get; set; } = null!;
    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
}

