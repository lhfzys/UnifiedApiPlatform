using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.AssignMenusToRole;

/// <summary>
/// 分配菜单给角色命令
/// </summary>
public class AssignMenusToRoleCommand : CommandBase
{
    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 菜单 ID 列表
    /// </summary>
    public List<Guid> MenuIds { get; set; } = new();
}
