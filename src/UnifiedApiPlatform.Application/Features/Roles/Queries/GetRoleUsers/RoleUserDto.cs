namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleUsers;

public class RoleUserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Avatar { get; set; }
    public bool IsActive { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime AssignedAt { get; set; }  // 分配时间
}
