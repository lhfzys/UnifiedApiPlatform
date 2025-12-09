using NodaTime;

namespace UnifiedApiPlatform.Domain.Common;

/// <summary>
/// 可审计实体基类
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public Instant? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }
}
