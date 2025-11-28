using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 实体附件关联
/// </summary>
public class EntityAttachment : BaseEntity
{
    public string TenantId { get; set; } = null!;
    public string EntityType { get; set; } = null!; // Order/User/Report
    public string EntityId { get; set; } = null!;
    public Guid FileId { get; set; }
    public FileRecord File { get; set; } = null!;
    public string? AttachmentType { get; set; } // Avatar/Document/Image/Contract
    public string? Title { get; set; }
    public int Sort { get; set; }
    public bool IsMain { get; set; } // 是否主图
    public string UploadedBy { get; set; } = null!;
    public Instant UploadedAt { get; set; }
}
