namespace UnifiedApiPlatform.Shared.Attributes;

/// <summary>
/// 幂等性特性标记
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IdempotentAttribute : Attribute
{
    /// <summary>
    /// 过期时间（小时）
    /// </summary>
    public int ExpirationHours { get; set; } = 24;
}
