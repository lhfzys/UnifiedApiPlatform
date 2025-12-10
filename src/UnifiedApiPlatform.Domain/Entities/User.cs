using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : MultiTenantEntity, IAggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = [];

    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public bool IsActive { get; set; } = true;
    public Instant? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public Instant? PasswordChangedAt { get; set; }
    public Instant? LockedUntil { get; set; }
    public int LoginFailureCount { get; set; }
    public bool IsSystemUser { get; set; } = false;

    // 组织关联
    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    // 上级领导
    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }

    // 导航属性
    public Tenant Tenant { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // 领域事件
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    // ==================== 业务方法 ====================

    /// <summary>
    /// 修改密码
    /// </summary>
    public void ChangePassword(string newPasswordHash, IClock clock)
    {
        PasswordHash = newPasswordHash;
        PasswordChangedAt = clock.GetCurrentInstant();
    }

    /// <summary>
    /// 锁定账户（指定分钟数）
    /// </summary>
    public void LockAccount(IClock clock, int minutes = 15)
    {
        LockedUntil = clock.GetCurrentInstant().Plus(Duration.FromMinutes(minutes));
    }

    /// <summary>
    /// 解锁账户
    /// </summary>
    public void UnlockAccount()
    {
        LockedUntil = null;
        LoginFailureCount = 0;
    }

    /// <summary>
    /// 记录登录成功
    /// </summary>
    public void RecordLoginSuccess(string ipAddress, IClock clock)
    {
        LastLoginAt = clock.GetCurrentInstant();
        LastLoginIp = ipAddress;
        LoginFailureCount = 0;
        UnlockAccount();
    }

    /// <summary>
    /// 记录登录失败
    /// </summary>
    public void RecordLoginFailure(IClock clock)
    {
        LoginFailureCount++;

        // 连续失败 5 次，锁定 15 分钟
        if (LoginFailureCount >= 5)
        {
            LockAccount(clock, 15);  // 参数顺序：clock, minutes
        }
    }

    /// <summary>
    /// 是否被锁定（计算属性）
    /// </summary>
    public bool IsLocked => LockedUntil.HasValue && LockedUntil.Value > Instant.FromDateTimeUtc(DateTime.UtcNow);

    /// <summary>
    /// 激活账户
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        Status = UserStatus.Active;
    }

    /// <summary>
    /// 停用账户
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        Status = UserStatus.Inactive;
    }
}
