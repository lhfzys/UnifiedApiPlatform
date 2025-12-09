namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class AssignPermissionsRequest
{
    public Guid RoleId { get; set; }
    public List<string> PermissionCodes { get; set; } = new();
}
