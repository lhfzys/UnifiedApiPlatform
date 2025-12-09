using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionCode });

        builder.Property(rp => rp.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);

        // 配置审计字段
        builder.Property(rp => rp.CreatedAt).IsRequired();
        builder.Property(rp => rp.CreatedBy).HasMaxLength(50);
        builder.Property(rp => rp.UpdatedAt);
        builder.Property(rp => rp.UpdatedBy).HasMaxLength(50);

        builder.Property(rp => rp.RowVersion).IsRowVersion();
        // 索引
        builder.HasIndex(rp => rp.RoleId)
            .HasDatabaseName("ix_role_permissions_role_id");

        builder.HasIndex(rp => rp.PermissionCode)
            .HasDatabaseName("ix_role_permissions_permission_code");

        // 关系配置
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionCode)
            .HasPrincipalKey(p => p.Code)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
