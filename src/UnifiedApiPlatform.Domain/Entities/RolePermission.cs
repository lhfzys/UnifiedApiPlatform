using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 角色权限关联
/// </summary>
public class RolePermission : AuditableEntity
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public string PermissionCode { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
