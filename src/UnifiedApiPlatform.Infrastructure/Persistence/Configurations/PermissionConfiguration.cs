using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.IsSystemPermission)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        // 配置审计字段
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(50);
        builder.Property(p => p.UpdatedAt);
        builder.Property(p => p.UpdatedBy).HasMaxLength(50);

        // 软删除
        builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(p => p.DeletedAt);
        builder.Property(p => p.DeletedBy).HasMaxLength(50);


        builder.HasIndex(p => p.Category)
            .HasDatabaseName("ix_permissions_category");

        // 全局查询过滤器（软删除）
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
