using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 租户实体
/// </summary>
public class Tenant: SoftDeletableEntity, IAggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = new();

    public string Identifier { get; set; } = null!; // 租户唯一标识
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public Instant? ActivatedAt { get; set; }
    public Instant? SuspendedAt { get; set; }
    public string? SuspendedReason { get; set; }

    // ==================== 配额信息 ====================
    /// <summary>
    /// 最大用户数
    /// </summary>
    public int MaxUsers { get; set; } = 100;

    /// <summary>
    /// 最大存储空间（字节）
    /// </summary>
    public long MaxStorageInBytes { get; set; } = 10L * 1024 * 1024 * 1024; // 10GB

    /// <summary>
    /// 已使用存储空间（字节）
    /// </summary>
    public long StorageUsedInBytes { get; set; }

    /// <summary>
    /// 每日最大 API 调用次数
    /// </summary>
    public int MaxApiCallsPerDay { get; set; } = 100000;

    // ==================== 联系信息 ====================
    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系人邮箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系人地址
    /// </summary>
    public string? Address { get; set; }

    // ==================== 导航属性 ====================
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

    // ==================== 业务方法 ====================
    /// <summary>
    /// 激活租户
    /// </summary>
    public void Activate(IClock clock)
    {
        IsActive = true;
        ActivatedAt = clock.GetCurrentInstant();
        SuspendedAt = null;
        SuspendedReason = null;
    }

    /// <summary>
    /// 停用租户
    /// </summary>
    public void Suspend(string reason, IClock clock)
    {
        IsActive = false;
        SuspendedAt = clock.GetCurrentInstant();
        SuspendedReason = reason;
    }

    /// <summary>
    /// 检查是否可以添加用户
    /// </summary>
    public bool CanAddUser(int currentUserCount)
    {
        return currentUserCount < MaxUsers;
    }

    /// <summary>
    /// 检查是否可以上传文件
    /// </summary>
    public bool CanUploadFile(long fileSize)
    {
        return (StorageUsedInBytes + fileSize) <= MaxStorageInBytes;
    }

    /// <summary>
    /// 增加存储使用量
    /// </summary>
    public void IncreaseStorageUsage(long bytes)
    {
        StorageUsedInBytes += bytes;
    }

    /// <summary>
    /// 减少存储使用量
    /// </summary>
    public void DecreaseStorageUsage(long bytes)
    {
        StorageUsedInBytes = Math.Max(0, StorageUsedInBytes - bytes);
    }

    /// <summary>
    /// 检查是否为默认租户
    /// </summary>
    public bool IsDefaultTenant()
    {
        return Identifier.Equals("default", StringComparison.OrdinalIgnoreCase);
    }
}
