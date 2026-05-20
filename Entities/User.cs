namespace JirafeAPI.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = new List<WorkspaceMember>();
    public virtual ICollection<BoardMember> BoardMembers { get; set; } = new List<BoardMember>();
    public virtual ICollection<CardMember> CardMembers { get; set; } = new List<CardMember>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}