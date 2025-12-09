namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class GetOrganizationsRequest
{
    public string? SearchKeyword { get; set; }
    public Guid? ParentId { get; set; }
    public bool? IsActive { get; set; }
    public bool AsTree { get; set; }
}
