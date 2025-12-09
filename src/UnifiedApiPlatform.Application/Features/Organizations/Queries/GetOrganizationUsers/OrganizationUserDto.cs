namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationUsers;

public class OrganizationUserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Avatar { get; set; }
    public bool IsActive { get; set; }
    public string? ManagerName { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
