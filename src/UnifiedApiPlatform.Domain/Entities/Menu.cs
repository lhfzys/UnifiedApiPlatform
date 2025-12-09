using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class Menu : AuditableEntity,IAggregateRoot
{
    /// <summary>
    /// 租户 ID（系统菜单为 null）
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// 父菜单 ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单代码（唯一标识，如 "users"）
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// 菜单名称（显示名称，如"用户管理"）
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuType Type { get; set; }

    /// <summary>
    /// 关联的权限代码
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径（前端组件）
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 是否系统菜单（系统菜单不可删除）
    /// </summary>
    public bool IsSystemMenu { get; set; }

    // 导航属性
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
    public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

    // 领域事件
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent eventItem) => _domainEvents.Add(eventItem);
    public void RemoveDomainEvent(DomainEvent eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
