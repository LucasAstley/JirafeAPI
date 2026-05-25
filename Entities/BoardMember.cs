namespace JirafeAPI.Entities;

public class BoardMember
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; }

    public virtual Board Board { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

