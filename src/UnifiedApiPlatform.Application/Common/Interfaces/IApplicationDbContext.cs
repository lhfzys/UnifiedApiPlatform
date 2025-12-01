using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // DbSets
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    public DbSet<Permission> Permissions { get; }
    public DbSet<Menu> Menus { get; }
    public DbSet<Organization> Organizations { get; }
    public DbSet<UserRole> UserRoles { get; }
    public DbSet<RolePermission> RolePermissions { get; }
    public DbSet<RoleMenu> RoleMenus { get; }
    public DbSet<DataScope> DataScopes { get; }
    public DbSet<RefreshToken> RefreshTokens { get; }
    public DbSet<AuditLog> AuditLogs { get; }
    public DbSet<OperationLog> OperationLogs { get; }
    public DbSet<LoginLog> LoginLogs { get; }
    public DbSet<FileRecord> FileRecords { get; }
    public DbSet<EntityAttachment> EntityAttachments { get; }
    public DbSet<Notification> Notifications { get; }
    public DbSet<NotificationRecipient> NotificationRecipients { get; }
    public DbSet<UserNotificationSettings> UserNotificationSettings { get; }
    public DbSet<DictionaryCategory> DictionaryCategories { get; }
    public DbSet<DictionaryItem> DictionaryItems { get; }
    public DbSet<SystemSettings> SystemSettings { get; }
    public DbSet<TenantSettings> TenantSettings { get; }
    public DbSet<Announcement> Announcements { get; }
    public DbSet<AnnouncementRead> AnnouncementReads { get; }
    public DbSet<ImportJob> ImportJobs { get; }
    public DbSet<ImportJobDetail> ImportJobDetails { get; }
    public DbSet<ScheduledJob> ScheduledJobs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
