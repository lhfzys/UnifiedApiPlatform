using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 租户配置
/// </summary>
public class TenantSettings : BaseEntity
{
    public string TenantId { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public string DataType { get; set; } = "String";
    public bool InheritFromSystem { get; set; } = true;
    public Instant UpdatedAt { get; set; }
}
