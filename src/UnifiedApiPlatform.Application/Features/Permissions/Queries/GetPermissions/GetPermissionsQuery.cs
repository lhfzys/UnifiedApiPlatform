using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Permissions.Queries.GetPermissions;

/// <summary>
/// 获取权限列表查询
/// </summary>
public class GetPermissionsQuery : QueryBase<List<PermissionCategoryDto>>
{
    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public string? Category { get; set; }
}
