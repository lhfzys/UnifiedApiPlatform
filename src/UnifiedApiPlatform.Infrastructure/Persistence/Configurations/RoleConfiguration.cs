using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .HasColumnName("tenant_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.IsSystemRole)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.Sort)
            .HasColumnName("sort")
            .IsRequired()
            .HasDefaultValue(0);

        // 审计字段
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(r => r.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // 软删除字段
        builder.Property(r => r.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(r => r.DeletedBy)
            .HasColumnName("deleted_by")
            .HasMaxLength(100);

        builder.Property(r => r.RowVersion)
            .IsRowVersion();

        // 唯一索引（租户内角色代码唯一）
        builder.HasIndex(r => new { r.TenantId, r.Code })
            .IsUnique()
            .HasDatabaseName("ix_roles_tenant_code");

        // 其他索引
        builder.HasIndex(r => r.TenantId)
            .HasDatabaseName("ix_roles_tenant_id");

        builder.HasIndex(r => r.IsDeleted)
            .HasDatabaseName("ix_roles_is_deleted");

        // 关系配置
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RoleMenus)
            .WithOne(rm => rm.Role)
            .HasForeignKey(rm => rm.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
