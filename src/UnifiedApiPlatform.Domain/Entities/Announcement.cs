using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 公告
/// </summary>
public class Announcement : BaseEntity
{
    public string? TenantId { get; set; } // 系统公告为 null
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!; // 富文本
    public AnnouncementType Type { get; set; }
    public string Priority { get; set; } = "Medium"; // High/Medium/Low
    public Guid PublisherId { get; set; }
    public Instant PublishedAt { get; set; }
    public Instant? ExpiresAt { get; set; }
    public bool IsSticky { get; set; }
    public bool IsActive { get; set; } = true;
    public string TargetAudience { get; set; } = "All"; // All/Role/Organization
    public string? TargetRoleIds { get; set; } // JSON
    public string? TargetOrgIds { get; set; } // JSON
    public int ViewCount { get; set; }

    // 导航属性
    public ICollection<AnnouncementRead> ReadRecords { get; set; } = new List<AnnouncementRead>();
}
