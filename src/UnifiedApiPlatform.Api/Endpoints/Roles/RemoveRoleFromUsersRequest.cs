namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class RemoveRoleFromUsersRequest
{
    public Guid RoleId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
}
