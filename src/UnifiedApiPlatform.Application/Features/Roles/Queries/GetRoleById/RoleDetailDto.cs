using UnifiedApiPlatform.Application.Features.Permissions.Queries.GetPermissions;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleById;

public class RoleDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public int UserCount { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public string? RowVersion { get; set; }
}
