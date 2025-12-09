namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class CreateRoleRequest
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
