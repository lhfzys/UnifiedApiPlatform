using System.ComponentModel.DataAnnotations;

namespace UnifiedApiPlatform.Domain.Common;

/// <summary>
/// 实体基类
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 行版本（乐观并发控制）
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; } = [];
}
