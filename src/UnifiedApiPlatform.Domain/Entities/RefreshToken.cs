using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 刷新令牌
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string TenantId { get; set; } = null!;
    public string Token { get; set; } = null!; // SHA256 哈希
    public Instant ExpiresAt { get; set; }
    public Instant CreatedAt { get; set; }
    public string CreatedByIp { get; set; } = null!;
    public Instant? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? RevokeReason { get; set; }

    // 业务方法（不映射到数据库）
    public bool IsExpired(IClock clock) => clock.GetCurrentInstant() >= ExpiresAt;
    public bool IsRevoked() => RevokedAt != null;
    public bool IsActive(IClock clock) => !IsRevoked() && !IsExpired(clock);
}
