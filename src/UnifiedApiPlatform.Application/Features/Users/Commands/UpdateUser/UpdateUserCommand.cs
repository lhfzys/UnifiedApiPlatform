using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : CommandBase<UpdateUserResponse>
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
    public byte[]? RowVersion { get; set; }
}

/// <summary>
/// 更新用户响应
/// </summary>
public class UpdateUserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
}
