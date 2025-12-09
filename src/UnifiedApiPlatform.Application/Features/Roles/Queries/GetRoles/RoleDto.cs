namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoles;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public int UserCount { get; set; }  // 用户数量
    public int PermissionCount { get; set; }  // 权限数量
    public DateTime CreatedAt { get; set; }
}
