namespace UnifiedApiPlatform.Shared.Models;

/// <summary>
/// 权限定义
/// </summary>
public record PermissionDefinition
{
    /// <summary>
    /// 权限代码（唯一标识）
    /// </summary>
    public string Code { get; init; } = null!;

    /// <summary>
    /// 权限名称（显示名称）
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// 分类
    /// </summary>
    public string Category { get; init; } = null!;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; init; }

    public PermissionDefinition(
        string code,
        string name,
        string category,
        string? description = null,
        int sortOrder = 0)
    {
        Code = code;
        Name = name;
        Category = category;
        Description = description;
        SortOrder = sortOrder;
    }
}
