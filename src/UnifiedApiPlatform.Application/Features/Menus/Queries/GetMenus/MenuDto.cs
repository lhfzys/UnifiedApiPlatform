using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

/// <summary>
/// 菜单 DTO
/// </summary>
public class MenuDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public MenuType Type { get; set; }
    public string? PermissionCode { get; set; }
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemMenu { get; set; }
    public int ChildCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 菜单树节点 DTO
/// </summary>
public class MenuTreeNodeDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public MenuType Type { get; set; }
    public string? PermissionCode { get; set; }
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public bool IsActive { get; set; }
    public List<MenuTreeNodeDto> Children { get; set; } = new();
}
