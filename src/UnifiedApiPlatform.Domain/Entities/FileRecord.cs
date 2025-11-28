using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 文件记录
/// </summary>
public class FileRecord : MultiTenantEntity
{
    public string FileName { get; set; } = null!;
    public string StorageKey { get; set; } = null!; // 存储路径/Key
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public string? FileHash { get; set; } // SHA256（用于去重）
    public FileCategory Category { get; set; }
    public string UploadedBy { get; set; } = null!;
    public Instant UploadedAt { get; set; }
    public Instant? ExpiresAt { get; set; } // 临时文件过期时间
    public bool IsPublic { get; set; }
    public string? Metadata { get; set; } // JSON 扩展元数据

    // 导航属性
    public ICollection<EntityAttachment> EntityAttachments { get; set; } = new List<EntityAttachment>();
}
