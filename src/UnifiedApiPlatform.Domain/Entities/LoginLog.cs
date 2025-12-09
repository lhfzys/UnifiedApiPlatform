using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 登录日志
/// </summary>
public class LoginLog : BaseEntity
{
    /// <summary>
    /// 租户 ID
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// 用户 ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 登录类型（Login/Logout/RefreshToken）
    /// </summary>
    public string LoginType { get; set; } = null!;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 失败原因
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// IP 地址
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// User-Agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// 设备类型（Desktop/Mobile/Tablet）
    /// </summary>
    public string? DeviceType { get; set; }

    /// <summary>
    /// 地理位置（国家/城市）
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public Instant CreatedAt { get; set; }

    // 导航属性
    public User? User { get; set; }
}
