namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class AssignUsersToOrganizationRequest
{
    public Guid OrganizationId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
}
