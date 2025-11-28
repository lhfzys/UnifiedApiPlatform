using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 数据权限范围
/// </summary>
public class DataScope : BaseEntity
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public string ResourceType { get; set; } = null!; // User/Order/Report
    public DataScopeType ScopeType { get; set; }
    public string? ScopeValue { get; set; } // JSON 格式
    public string? FilterExpression { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
