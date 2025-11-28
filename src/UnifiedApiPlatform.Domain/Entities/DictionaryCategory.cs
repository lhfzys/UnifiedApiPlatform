using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 字典分类
/// </summary>
public class DictionaryCategory : BaseEntity
{
    public string? TenantId { get; set; } // 系统字典为 null
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public bool IsEditable { get; set; } = true;
    public int Sort { get; set; }
    public Instant CreatedAt { get; set; }

    // 导航属性
    public ICollection<DictionaryItem> Items { get; set; } = new List<DictionaryItem>();
}
