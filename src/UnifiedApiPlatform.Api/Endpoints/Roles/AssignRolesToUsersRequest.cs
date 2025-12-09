namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class AssignRolesToUsersRequest
{
    public Guid RoleId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
}
