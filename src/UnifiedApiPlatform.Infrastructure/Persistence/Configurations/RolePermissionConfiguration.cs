using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        // 主键
        builder.HasKey(rp => rp.Id);

        // RoleId
        builder.Property(rp => rp.RoleId)
            .HasColumnName("role_id")
            .IsRequired();

        // PermissionId（外键）
        builder.Property(rp => rp.PermissionId)
            .HasColumnName("permission_id")
            .IsRequired();

        // 配置审计字段
        builder.Property(rp => rp.CreatedAt).IsRequired();
        builder.Property(rp => rp.CreatedBy).HasMaxLength(50);
        builder.Property(rp => rp.UpdatedAt);
        builder.Property(rp => rp.UpdatedBy).HasMaxLength(50);

        builder.Property(rp => rp.RowVersion).IsRowVersion();

        // 唯一索引（角色-权限唯一）
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique()
            .HasDatabaseName("ix_role_permissions_role_permission");

        // 外键索引
        builder.HasIndex(rp => rp.RoleId)
            .HasDatabaseName("ix_role_permissions_role_id");

        builder.HasIndex(rp => rp.PermissionId)
            .HasDatabaseName("ix_role_permissions_permission_id");

        // 关系配置
        // 导航属性配置
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
