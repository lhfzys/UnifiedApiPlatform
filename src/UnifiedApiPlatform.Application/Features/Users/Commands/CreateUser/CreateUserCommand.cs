using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand: CommandBase<CreateUserResponse>
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

public class CreateUserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
