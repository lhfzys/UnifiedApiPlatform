using NodaTime;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 角色菜单关联
/// </summary>
public class RoleMenu
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public Guid MenuId { get; set; }
    public Menu Menu { get; set; } = null!;

    public Instant CreatedAt { get; set; } = SystemClock.Instance.GetCurrentInstant();
}
