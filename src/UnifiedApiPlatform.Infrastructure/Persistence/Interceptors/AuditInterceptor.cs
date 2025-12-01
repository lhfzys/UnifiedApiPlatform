using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Domain.Enums;
using UnifiedApiPlatform.Shared.Helpers;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Interceptors;

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

        var now = _clock.GetCurrentInstant();
        var userId = _currentUser.IsAuthenticated ? _currentUser.UserId : null;
        var userName = _currentUser.IsAuthenticated ? _currentUser.UserName : "System";
        var tenantId = _currentUser.IsAuthenticated ? _currentUser.TenantId : "system";

        var auditLogs = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // 跳过不需要审计的实体
            if (ShouldSkipAudit(entry))
                continue;

            // 跳过未修改的实体
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            // 获取实体的 TenantId
            var entityTenantId = GetTenantId(entry.Entity, tenantId);

            var auditLog = new AuditLog
            {
                TenantId = entityTenantId,
                UserId = userId != null ? Guid.Parse(userId).ToString() : null,
                UserName = userName,
                EntityType = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKey(entry)?.ToString(),
                Action = MapEntityStateToAction(entry.State),
                Timestamp = now
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditLog.NewValues = GetNewValues(entry);
                    break;

                case EntityState.Modified:
                    auditLog.OldValues = GetOldValues(entry);
                    auditLog.NewValues = GetNewValues(entry);
                    auditLog.ChangedProperties = GetChangedProperties(entry);
                    break;

                case EntityState.Deleted:
                    auditLog.OldValues = GetOldValues(entry);
                    break;
            }

            auditLogs.Add(auditLog);
        }

        if (auditLogs.Any())
        {
            context.Set<AuditLog>().AddRange(auditLogs);
        }
    }

    private static bool ShouldSkipAudit(EntityEntry entry)
    {
        var entityType = entry.Entity.GetType();

        // 跳过审计日志本身和其他日志表
        return entityType == typeof(AuditLog) ||
               entityType == typeof(OperationLog) ||
               entityType == typeof(LoginLog) ||
               entityType == typeof(RefreshToken);
    }

    private static string GetTenantId(object entity, string defaultTenantId)
    {
        // 尝试从 MultiTenantEntity 获取 TenantId
        if (entity is MultiTenantEntity multiTenantEntity)
        {
            return multiTenantEntity.TenantId;
        }

        // 尝试从其他实体获取 TenantId（如 User, Role 等）
        var tenantIdProperty = entity.GetType().GetProperty("TenantId");
        if (tenantIdProperty != null)
        {
            var value = tenantIdProperty.GetValue(entity);
            if (value is string tenantId && !string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }
        }

        // Tenant 实体本身，使用其 Identifier 作为 TenantId
        if (entity is Tenant tenant)
        {
            return tenant.Identifier;
        }

        // 默认使用 system（用于系统级操作，如租户创建、权限管理等）
        return defaultTenantId;
    }

    private static AuditAction MapEntityStateToAction(EntityState state)
    {
        return state switch
        {
            EntityState.Added => AuditAction.Create,
            EntityState.Modified => AuditAction.Update,
            EntityState.Deleted => AuditAction.Delete,
            _ => AuditAction.Update
        };
    }

    private static object? GetPrimaryKey(EntityEntry entry)
    {
        var keyName = entry.Metadata.FindPrimaryKey()?.Properties
            .Select(x => x.Name)
            .FirstOrDefault();

        return keyName != null ? entry.Property(keyName).CurrentValue : null;
    }

    private static string? GetOldValues(EntityEntry entry)
    {
        var properties = entry.Properties
            .Where(p => p.Metadata.ClrType != typeof(byte[]) && !p.Metadata.IsPrimaryKey())
            .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);

        return properties.Any() ? JsonHelper.Serialize(properties) : null;
    }

    private static string? GetNewValues(EntityEntry entry)
    {
        var properties = entry.Properties
            .Where(p => p.Metadata.ClrType != typeof(byte[]) && !p.Metadata.IsPrimaryKey())
            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

        return properties.Any() ? JsonHelper.Serialize(properties) : null;
    }

    private static string GetChangedProperties(EntityEntry entry)
    {
        return string.Join(", ", entry.Properties
            .Where(p => p.IsModified && !p.Metadata.IsPrimaryKey())
            .Select(p => p.Metadata.Name));
    }
}
