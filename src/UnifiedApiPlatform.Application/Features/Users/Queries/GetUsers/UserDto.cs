using NodaTime;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// 用户列表 DTO
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public Instant? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public Instant CreatedAt { get; set; }

    // 关联信息
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public List<string> Roles { get; set; } = [];
}
