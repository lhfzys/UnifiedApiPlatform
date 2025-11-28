using NodaTime;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 公告已读记录
/// </summary>
public class AnnouncementRead
{
    public Guid AnnouncementId { get; set; }
    public Announcement Announcement { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Instant ReadAt { get; set; }
}
