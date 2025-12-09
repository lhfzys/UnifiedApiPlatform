namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class GetOrganizationUsersRequest
{
    public Guid OrganizationId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public string? SearchKeyword { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeChildren { get; set; }
}
