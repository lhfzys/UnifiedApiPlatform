using System.ComponentModel.DataAnnotations;

namespace UnifiedApiPlatform.Domain.Common;

public abstract class MultiTenantEntity : SoftDeletableEntity
{
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// 行版本（乐观并发控制）
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
