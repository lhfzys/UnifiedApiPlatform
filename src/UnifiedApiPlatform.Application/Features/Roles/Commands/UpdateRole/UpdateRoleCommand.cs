using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommand : CommandBase<UpdateRoleResponse>
{
    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 行版本（乐观并发控制）
    /// </summary>
    public byte[]? RowVersion { get; set; }
}

public class UpdateRoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}
