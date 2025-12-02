using UnifiedApiPlatform.Application.Common.Queries;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// 获取用户列表查询
/// </summary>
public class GetUsersQuery: PagedQueryBase<UserDto>
{
    /// <summary>
    /// 搜索关键字（用户名、邮箱、手机号）
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 用户状态筛选
    /// </summary>
    public UserStatus? Status { get; set; }

    /// <summary>
    /// 是否激活筛选
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// 组织 ID 筛选
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// 角色 ID 筛选
    /// </summary>
    public Guid? RoleId { get; set; }
}
