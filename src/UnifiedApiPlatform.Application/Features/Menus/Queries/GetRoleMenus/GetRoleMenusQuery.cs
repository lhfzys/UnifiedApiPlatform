using UnifiedApiPlatform.Application.Common.Queries;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetRoleMenus;

/// <summary>
/// 获取角色菜单查询
/// </summary>
public class GetRoleMenusQuery : QueryBase<List<MenuTreeNodeDto>>
{
    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid RoleId { get; set; }
}
