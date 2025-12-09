using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommand : CommandBase<CreateRoleResponse>
{
    /// <summary>
    /// 角色名称（唯一标识）
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 权限代码列表
    /// </summary>
    public List<string>? PermissionCodes { get; set; }
}

public class CreateRoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}
