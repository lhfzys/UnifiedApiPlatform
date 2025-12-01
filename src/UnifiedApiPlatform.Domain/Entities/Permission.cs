using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 权限实体
/// </summary>
public class Permission : BaseEntity
{
    public string? TenantId { get; set; } // 系统权限为 null
    public string Code { get; set; } = null!; // 唯一标识，如 users.create
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsSystemPermission { get; set; }
    public Instant CreatedAt { get; set; }

    // 导航属性
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
