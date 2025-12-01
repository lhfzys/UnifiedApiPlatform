namespace UnifiedApiPlatform.Shared.Attributes;

/// <summary>
/// 缓存特性标记
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CachedAttribute : Attribute
{
    /// <summary>
    /// 缓存键
    /// </summary>
    public string? CacheKey { get; set; }

    /// <summary>
    /// 过期时间（秒）
    /// </summary>
    public int ExpirationSeconds { get; set; } = 300; // 默认 5 分钟

    /// <summary>
    /// 是否滑动过期
    /// </summary>
    public bool SlidingExpiration { get; set; } = false;
}
