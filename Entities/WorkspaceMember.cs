namespace JirafeAPI.Entities;

public class WorkspaceMember
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }

    public virtual Workspace Workspace { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

