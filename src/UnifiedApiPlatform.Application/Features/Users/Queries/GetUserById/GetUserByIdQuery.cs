using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// 根据 ID 获取用户详情查询
/// </summary>
public class GetUserByIdQuery: QueryBase<UserDetailDto>
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public Guid UserId { get; set; }
}
