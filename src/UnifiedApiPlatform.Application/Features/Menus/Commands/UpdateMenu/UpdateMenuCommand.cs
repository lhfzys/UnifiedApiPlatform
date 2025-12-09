using UnifiedApiPlatform.Application.Common.Commands;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.UpdateMenu;

/// <summary>
/// 更新菜单命令
/// </summary>
public class UpdateMenuCommand : CommandBase<UpdateMenuResponse>
{
    /// <summary>
    /// 菜单 ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 父菜单 ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuType? Type { get; set; }

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
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool? IsVisible { get; set; }

    /// <summary>
    /// 行版本
    /// </summary>
    public byte[]? RowVersion { get; set; }
}

/// <summary>
/// 更新菜单响应
/// </summary>
public class UpdateMenuResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
