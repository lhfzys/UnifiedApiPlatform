using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenuById;

/// <summary>
/// 菜单详情 DTO
/// </summary>
public class MenuDetailDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
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
    public int RoleCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public string? RowVersion { get; set; }
}
