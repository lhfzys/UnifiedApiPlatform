using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetUserRoles;

public class UserRoleDto
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTime AssignedAt { get; set; }
}
