using UnifiedApiPlatform.Application.Common.Queries;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenuTree;

/// <summary>
/// 获取菜单树查询
/// </summary>
public class GetMenuTreeQuery : QueryBase<List<MenuTreeNodeDto>>
{
    /// <summary>
    /// 是否包含未激活的菜单
    /// </summary>
    public bool IncludeInactive { get; set; }

    /// <summary>
    /// 是否包含不可见的菜单
    /// </summary>
    public bool IncludeHidden { get; set; }
}
