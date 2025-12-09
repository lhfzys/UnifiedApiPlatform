using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

public class GetMenusRequest
{
    public string? SearchKeyword { get; set; }
    public Guid? ParentId { get; set; }
    public MenuType? Type { get; set; }
    public bool? IsVisible { get; set; }
    public bool? IsActive { get; set; }
}
