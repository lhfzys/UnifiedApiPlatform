using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationUsers;

/// <summary>
/// 获取组织用户列表查询
/// </summary>
public class GetOrganizationUsersQuery : PagedQueryBase<OrganizationUserDto>
{
    /// <summary>
    /// 组织 ID
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// 搜索关键字（用户名、邮箱）
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// 是否包含子组织的用户
    /// </summary>
    public bool IncludeChildren { get; set; }
}
