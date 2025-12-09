using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.DeleteMenu;

/// <summary>
/// 删除菜单命令
/// </summary>
public class DeleteMenuCommand : CommandBase
{
    /// <summary>
    /// 菜单 ID
    /// </summary>
    public Guid MenuId { get; set; }
}
