namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class UpdateRoleRequest
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
    /// 行版本（Base64 编码）
    /// </summary>
    public string? RowVersion { get; set; }
}
