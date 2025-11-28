using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 租户实体
/// </summary>
public class Tenant: AuditableEntity, IAggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = new();

    public string Identifier { get; set; } = null!; // 租户唯一标识
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public Instant? ActivatedAt { get; set; }
    public Instant? SuspendedAt { get; set; }
    public string? SuspendedReason { get; set; }

    // 配额信息
    public int MaxUsers { get; set; } = 100;
    public long MaxStorageInBytes { get; set; } = 10L * 1024 * 1024 * 1024; // 10GB
    public long StorageUsedInBytes { get; set; }
    public int MaxApiCallsPerDay { get; set; } = 100000;

    // 联系信息
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    // 导航属性
    public ICollection<User> Users { get; set; } = new List<User>();

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
    public void Activate(IClock clock)
    {
        IsActive = true;
        ActivatedAt = clock.GetCurrentInstant();
        SuspendedAt = null;
        SuspendedReason = null;
    }

    public void Suspend(string reason, IClock clock)
    {
        IsActive = false;
        SuspendedAt = clock.GetCurrentInstant();
        SuspendedReason = reason;
    }

    public bool CanAddUser(int currentUserCount)
    {
        return currentUserCount < MaxUsers;
    }

    public bool CanUploadFile(long fileSize)
    {
        return (StorageUsedInBytes + fileSize) <= MaxStorageInBytes;
    }

    public void IncreaseStorageUsage(long bytes)
    {
        StorageUsedInBytes += bytes;
    }

    public void DecreaseStorageUsage(long bytes)
    {
        StorageUsedInBytes = Math.Max(0, StorageUsedInBytes - bytes);
    }
}
