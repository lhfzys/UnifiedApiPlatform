namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class UpdateOrganizationRequest
{
    public Guid OrganizationId { get; set; }
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
    public string? FullName { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public Guid? ManagerId { get; set; }
    public int? SortOrder { get; set; }
    public string? RowVersion { get; set; }
}
