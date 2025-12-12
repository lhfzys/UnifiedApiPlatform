using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        // 主键
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasMaxLength(50)
            .IsRequired();

        // 租户标识（唯一）
        builder.Property(t => t.Identifier)
            .HasColumnName("identifier")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.Identifier)
            .IsUnique()
            .HasDatabaseName("ix_tenants_identifier");

        // 租户名称
        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        // 描述
        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        // 是否激活
        builder.Property(t => t.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        // 激活时间
        builder.Property(t => t.ActivatedAt)
            .HasColumnName("activated_at");

        // 停用时间
        builder.Property(t => t.SuspendedAt)
            .HasColumnName("suspended_at");

        // 停用原因
        builder.Property(t => t.SuspendedReason)
            .HasColumnName("suspended_reason")
            .HasMaxLength(500);

        // 配额信息
        builder.Property(t => t.MaxUsers)
            .HasColumnName("max_users")
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(t => t.MaxStorageInBytes)
            .HasColumnName("max_storage_in_bytes")
            .IsRequired()
            .HasDefaultValue(10L * 1024 * 1024 * 1024);

        builder.Property(t => t.StorageUsedInBytes)
            .HasColumnName("storage_used_in_bytes")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.MaxApiCallsPerDay)
            .HasColumnName("max_api_calls_per_day")
            .IsRequired()
            .HasDefaultValue(100000);

        // 联系信息
        builder.Property(t => t.ContactName)
            .HasColumnName("contact_name")
            .HasMaxLength(100);

        builder.Property(t => t.ContactEmail)
            .HasColumnName("contact_email")
            .HasMaxLength(100);

        builder.Property(t => t.ContactPhone)
            .HasColumnName("contact_phone")
            .HasMaxLength(50);

        // 审计字段
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(t => t.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        //  软删除字段
        builder.Property(t => t.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(t => t.DeletedBy)
            .HasColumnName("deleted_by")
            .HasMaxLength(100);

        // RowVersion
        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        // 索引
        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("ix_tenants_is_active");

        builder.HasIndex(t => t.IsDeleted)
            .HasDatabaseName("ix_tenants_is_deleted");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("ix_tenants_created_at");

        // 导航属性
        builder.HasMany(t => t.Users)
            .WithOne()
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // 忽略领域事件（不映射到数据库）
        builder.Ignore(t => t.DomainEvents);

    }
}
