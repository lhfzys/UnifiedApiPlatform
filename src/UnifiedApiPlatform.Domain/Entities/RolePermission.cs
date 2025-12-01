using NodaTime;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 角色权限关联
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public string PermissionCode { get; set; } = null!;
    public Permission Permission { get; set; } = null!;

    public Instant CreatedAt { get; set; }
}
