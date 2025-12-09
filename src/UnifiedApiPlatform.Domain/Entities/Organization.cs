using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 组织（聚合根）
/// </summary>
public class Organization : MultiTenantEntity, IAggregateRoot
{
    /// <summary>
    /// 父组织 ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 组织编码（唯一标识）
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// 组织名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 完整名称
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 组织类型（Company/Department/Team）
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 负责人 ID
    /// </summary>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// 层级（从 1 开始）
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 路径（如 /1/2/5）
    /// </summary>
    public string Path { get; set; } = null!;

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    // 导航属性
    public Organization? Parent { get; set; }
    public ICollection<Organization> Children { get; set; } = new List<Organization>();
    public User? Manager { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();

    // 领域事件
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent eventItem) => _domainEvents.Add(eventItem);
    public void RemoveDomainEvent(DomainEvent eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();

    // 业务方法

    /// <summary>
    /// 激活组织
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// 停用组织
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// 更新路径
    /// </summary>
    public void UpdatePath(string parentPath)
    {
        Path = string.IsNullOrEmpty(parentPath)
            ? $"/{Id}"
            : $"{parentPath}/{Id}";
    }

    /// <summary>
    /// 计算层级
    /// </summary>
    public void CalculateLevel()
    {
        Level = string.IsNullOrEmpty(Path) ? 1 : Path.Split('/', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
