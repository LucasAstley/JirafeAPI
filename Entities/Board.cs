namespace JirafeAPI.Entities;

public class Board
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int WorkspaceId { get; set; }

    public virtual Workspace Workspace { get; set; } = null!;
    public virtual ICollection<BoardMember> BoardMembers { get; set; } = new List<BoardMember>();
    public virtual ICollection<List> Lists { get; set; } = new List<List>();
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();
}

