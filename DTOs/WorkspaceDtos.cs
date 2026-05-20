namespace JirafeAPI.DTOs;

public class WorkspaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateWorkspaceRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateWorkspaceRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class WorkspaceMemberDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public class InviteWorkspaceMemberRequest
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Collaborator";
}

