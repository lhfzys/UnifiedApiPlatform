using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class Menu : BaseEntity
{
    public string? TenantId { get; set; } // 系统菜单为 null
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!; // 唯一标识
    public string Title { get; set; } = null!; // 显示名称
    public MenuType Type { get; set; }
    public string? PermissionCode { get; set; } // 关联权限
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public int Sort { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsSystemMenu { get; set; }
    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }

    // 导航属性
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
    public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
