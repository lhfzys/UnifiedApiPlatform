namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class GetRoleUsersRequest
{
    public Guid RoleId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public string? SearchKeyword { get; set; }
    public bool? IsActive { get; set; }
}
