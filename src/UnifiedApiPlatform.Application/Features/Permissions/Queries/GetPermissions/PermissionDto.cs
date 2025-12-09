namespace UnifiedApiPlatform.Application.Features.Permissions.Queries.GetPermissions;

/// <summary>
/// 权限 DTO
/// </summary>
public class PermissionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemPermission { get; set; }
}
/// <summary>
/// 权限分类 DTO
/// </summary>
public class PermissionCategoryDto
{
    public string Category { get; set; } = null!;
    public List<PermissionDto> Permissions { get; set; } = new();
}
