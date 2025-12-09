using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenuById;

/// <summary>
/// 获取菜单详情查询
/// </summary>
public class GetMenuByIdQuery : QueryBase<MenuDetailDto>
{
    public Guid MenuId { get; set; }
}
