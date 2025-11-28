using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : MultiTenantEntity, IAggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = new();

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
    public int FailedLoginAttempts { get; set; }

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

    // 业务方法
    public void ChangePassword(string newPasswordHash, IClock clock)
    {
        PasswordHash = newPasswordHash;
        PasswordChangedAt = clock.GetCurrentInstant();
    }

    public void LockAccount(Duration duration, IClock clock)
    {
        LockedUntil = clock.GetCurrentInstant().Plus(duration);
        ;
        Status = UserStatus.Locked;
    }

    public void UnlockAccount()
    {
        LockedUntil = null;
        FailedLoginAttempts = 0;
        Status = UserStatus.Active;
    }

    public void RecordLoginSuccess(string ipAddress, IClock clock)
    {
        LastLoginAt = clock.GetCurrentInstant();
        LastLoginIp = ipAddress;
        FailedLoginAttempts = 0;
    }

    public void RecordLoginFailure(IClock clock)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5)
        {
            LockAccount(Duration.FromMinutes(15), clock);
        }
    }

    public bool IsLocked(IClock clock)
    {
        return LockedUntil.HasValue && LockedUntil.Value > clock.GetCurrentInstant();
    }
}
