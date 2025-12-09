namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class CreateOrganizationRequest
{
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? FullName { get; set; }
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? ManagerId { get; set; }
    public int SortOrder { get; set; }
}
