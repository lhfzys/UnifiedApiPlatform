using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 字典项
/// </summary>
public class DictionaryItem : BaseEntity
{
    public Guid CategoryId { get; set; }
    public DictionaryCategory Category { get; set; } = null!;

    public string? TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string? Value { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool IsEnabled { get; set; } = true;
    public Guid? ParentId { get; set; } // 支持级联
    public string? ExtraData { get; set; } // JSON
    public Instant CreatedAt { get; set; }
}
