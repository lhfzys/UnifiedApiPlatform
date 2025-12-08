namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class UpdateUserRequest
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public bool? IsActive { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ManagerId { get; set; }
    public List<Guid>? RoleIds { get; set; }
    public string? RowVersion { get; set; }
}
