using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 用户角色关联
/// </summary>
public class UserRole : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
