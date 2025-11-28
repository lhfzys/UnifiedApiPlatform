using NodaTime;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 通知接收人
/// </summary>
public class NotificationRecipient
{
    public Guid NotificationId { get; set; }
    public Notification Notification { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // 状态
    public bool IsRead { get; set; }
    public Instant? ReadAt { get; set; }
    public bool IsArchived { get; set; }
}
