using NodaTime;

namespace UnifiedApiPlatform.Domain.Common;

/// <summary>
/// 支持软删除的实体基类
/// </summary>
public abstract class SoftDeletableEntity : AuditableEntity
{
    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    public string? DeletedBy { get; set; }
}
