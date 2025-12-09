using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.AssignUsersToOrganization;

/// <summary>
/// 批量分配用户到组织命令
/// </summary>
public class AssignUsersToOrganizationCommand : CommandBase
{
    /// <summary>
    /// 组织 ID
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// 用户 ID 列表
    /// </summary>
    public List<Guid> UserIds { get; set; } = new();
}
