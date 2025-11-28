using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 登录日志
/// </summary>
public class LoginLog : BaseEntity
{
    public string TenantId { get; set; } = null!;
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = null!; // 尝试登录的用户名
    public string LoginType { get; set; } = "Password"; // Password/RefreshToken/SSO
    public LoginStatus Status { get; set; }
    public string? FailureReason { get; set; }

    // 客户端信息
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceType { get; set; } // Web/Mobile/Desktop
    public string? Location { get; set; } // 根据 IP 解析的地理位置

    // 时间
    public Instant LoginAt { get; set; }
    public Instant? LogoutAt { get; set; }

    // 导航属性
    public User? User { get; set; }
}
