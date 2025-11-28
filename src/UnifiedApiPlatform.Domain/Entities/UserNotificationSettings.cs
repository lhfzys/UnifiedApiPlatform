using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 用户通知设置
/// </summary>
public class UserNotificationSettings : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string TenantId { get; set; } = null!;

    // 通知偏好
    public bool EnableRealtimeNotifications { get; set; } = true;
    public bool EnableEmailNotifications { get; set; } = true;

    // 分类通知设置（JSON）
    public string? CategorySettings { get; set; }

    // 免打扰时段（使用 LocalTime）
    public LocalTime? DoNotDisturbStart { get; set; }
    public LocalTime? DoNotDisturbEnd { get; set; }

    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }
}
