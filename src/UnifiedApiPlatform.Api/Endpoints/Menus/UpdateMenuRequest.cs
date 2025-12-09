using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

public class UpdateMenuRequest
{
    public Guid MenuId { get; set; }
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
    public MenuType? Type { get; set; }
    public string? PermissionCode { get; set; }
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsVisible { get; set; }
    public string? RowVersion { get; set; }
}
