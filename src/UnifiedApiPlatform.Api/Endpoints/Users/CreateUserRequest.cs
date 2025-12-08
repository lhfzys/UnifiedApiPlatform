namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class CreateUserRequest
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ManagerId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}
