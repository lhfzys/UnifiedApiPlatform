using NodaTime;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// 用户详情 DTO
/// </summary>
public class UserDetailDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Instant? LockedUntil { get; set; }
    public int LoginFailureCount { get; set; }
    public Instant? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public Instant? PasswordChangedAt { get; set; }
    public Instant CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public Instant? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 关联信息
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }

    // 角色和权限
    public List<RoleDto> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

/// <summary>
/// 角色 DTO
/// </summary>
public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
}
