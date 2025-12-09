using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 权限实体
/// </summary>
public class Permission: MultiTenantEntity, IAggregateRoot
{
    public string Code { get; set; } = null!; // 唯一标识，如 users.create
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemPermission { get; set; }
    // 导航属性
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(DomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
