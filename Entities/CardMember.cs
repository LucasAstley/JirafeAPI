namespace JirafeAPI.Entities;

public class CardMember
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public int UserId { get; set; }

    public virtual Card Card { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

