using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleById;

/// <summary>
/// 获取角色详情查询
/// </summary>
public class GetRoleByIdQuery : QueryBase<RoleDetailDto>
{
    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid RoleId { get; set; }
}
