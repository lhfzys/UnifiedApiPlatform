using UnifiedApiPlatform.Application.Common.Queries;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

/// <summary>
/// 获取菜单列表查询
/// </summary>
public class GetMenusQuery : QueryBase<List<MenuDto>>
{
    /// <summary>
    /// 搜索关键字（菜单名称、编码）
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 父菜单 ID（查询子菜单）
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuType? Type { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool? IsVisible { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool? IsActive { get; set; }
}
