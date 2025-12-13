using System.ComponentModel.DataAnnotations;

namespace UnifiedApiPlatform.Domain.Common;

/// <summary>
/// 多租户实体基类
/// </summary>
public abstract class MultiTenantEntity : SoftDeletableEntity
{
    public Guid TenantId { get; set; }

    /// <summary>
    /// 行版本（乐观并发控制）
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
