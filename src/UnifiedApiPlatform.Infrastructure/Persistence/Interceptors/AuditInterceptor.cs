using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Domain.Enums;
using UnifiedApiPlatform.Shared.Helpers;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// 审计拦截器 - 自动记录数据变更
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;

    public AuditInterceptor(ICurrentUserService currentUser, IClock clock)
    {
        _currentUser = currentUser;
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        CreateAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CreateAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void CreateAuditLogs(DbContext? context)
    {
        if (context == null) return;

        var auditEntries = new List<AuditLog>();
        var now = _clock.GetCurrentInstant();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // 跳过不需要审计的实体
            if (entry.Entity is AuditLog or OperationLog or LoginLog or RefreshToken)
                continue;

            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditLog = new AuditLog
            {
                TenantId = GetTenantId(entry.Entity),
                UserId = _currentUser.UserId,
                UserName = _currentUser.UserName,
                EntityType = entry.Entity.GetType().Name,
                EntityId = GetEntityId(entry.Entity),
                EntityName = GetEntityName(entry.Entity),
                Action = GetAuditAction(entry.State),
                Timestamp = now,
                Success = true
            };

            if (entry.State == EntityState.Added)
            {
                auditLog.NewValues = JsonHelper.Serialize(GetChangedProperties(entry));
            }
            else if (entry.State == EntityState.Modified)
            {
                auditLog.OldValues = JsonHelper.Serialize(GetOriginalValues(entry));
                auditLog.NewValues = JsonHelper.Serialize(GetChangedProperties(entry));
                auditLog.ChangedProperties = string.Join(",", entry.Properties
                    .Where(p => p.IsModified)
                    .Select(p => p.Metadata.Name));
            }
            else if (entry.State == EntityState.Deleted)
            {
                auditLog.OldValues = JsonHelper.Serialize(GetOriginalValues(entry));
            }

            auditEntries.Add(auditLog);
        }

        // 添加审计日志到数据库
        if (auditEntries.Any())
        {
            context.Set<AuditLog>().AddRange(auditEntries);
        }
    }

    private static string? GetTenantId(object entity)
    {
        return entity is MultiTenantEntity multiTenantEntity
            ? multiTenantEntity.TenantId
            : null;
    }

    private static string? GetEntityId(object entity)
    {
        return entity is BaseEntity baseEntity
            ? baseEntity.Id.ToString()
            : null;
    }

    private static string? GetEntityName(object entity)
    {
        // 尝试获取实体的 Name 属性
        var nameProperty = entity.GetType().GetProperty("Name");
        if (nameProperty != null)
        {
            return nameProperty.GetValue(entity)?.ToString();
        }

        var titleProperty = entity.GetType().GetProperty("Title");
        if (titleProperty != null)
        {
            return titleProperty.GetValue(entity)?.ToString();
        }

        return null;
    }

    private static AuditAction GetAuditAction(EntityState state)
    {
        return state switch
        {
            EntityState.Added => AuditAction.Create,
            EntityState.Modified => AuditAction.Update,
            EntityState.Deleted => AuditAction.Delete,
            _ => AuditAction.View
        };
    }

    private static Dictionary<string, object?> GetOriginalValues(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        return entry.Properties
            .Where(p => !p.Metadata.IsPrimaryKey())
            .ToDictionary(
                p => p.Metadata.Name,
                p => p.OriginalValue
            );
    }

    private static Dictionary<string, object?> GetChangedProperties(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        return entry.Properties
            .Where(p => !p.Metadata.IsPrimaryKey() && (entry.State == EntityState.Added || p.IsModified))
            .ToDictionary(
                p => p.Metadata.Name,
                p => p.CurrentValue
            );
    }
}
