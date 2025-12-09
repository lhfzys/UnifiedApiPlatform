using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

public class CreateMenuRequest
{
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public MenuType Type { get; set; }
    public string? PermissionCode { get; set; }
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
