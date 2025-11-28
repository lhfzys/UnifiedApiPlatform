using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 系统配置
/// </summary>
public class SystemSettings : BaseEntity
{
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public string DataType { get; set; } = "String"; // String/Int/Bool/Json
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsReadOnly { get; set; }
    public Instant UpdatedAt { get; set; }
}
