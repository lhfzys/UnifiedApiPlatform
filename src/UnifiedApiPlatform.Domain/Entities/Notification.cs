using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 通知
/// </summary>
public class Notification : BaseEntity
{
    public string TenantId { get; set; } = null!;

    // 内容
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public NotificationType Type { get; set; }
    public string? Category { get; set; }

    // 发送信息
    public Guid? SenderId { get; set; }
    public string? SenderName { get; set; }
    public Instant CreatedAt { get; set; }

    // 跳转链接
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }

    // 关联数据
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityId { get; set; }

    // 导航属性
    public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
}
