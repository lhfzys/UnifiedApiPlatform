namespace UnifiedApiPlatform.Api.Endpoints.Menus;

public class AssignMenusToRoleRequest
{
    public Guid RoleId { get; set; }
    public List<Guid> MenuIds { get; set; } = new();
}
