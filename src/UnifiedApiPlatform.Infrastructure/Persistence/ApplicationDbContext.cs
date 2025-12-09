using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

namespace UnifiedApiPlatform.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext,IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();
    public DbSet<FileRecord> FileRecords => Set<FileRecord>();
    public DbSet<EntityAttachment> EntityAttachments => Set<EntityAttachment>();
    public DbSet<DictionaryCategory> DictionaryCategories => Set<DictionaryCategory>();
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>();
    public DbSet<SystemSettings> SystemSettings => Set<SystemSettings>();
    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementRead> AnnouncementReads => Set<AnnouncementRead>();
    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
    public DbSet<ImportJobDetail> ImportJobDetails => Set<ImportJobDetail>();
    public DbSet<ScheduledJob> ScheduledJobs => Set<ScheduledJob>();
    // Host=localhost;Port=5432;Database=unifiedapi_db;Username=postgres;Password=postgres;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureGlobalFilters();
        // 应用所有实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // 配置默认命名约定：使用 snake_case
        configurationBuilder.Properties<string>().HaveMaxLength(255);

        base.ConfigureConventions(configurationBuilder);
    }
}
