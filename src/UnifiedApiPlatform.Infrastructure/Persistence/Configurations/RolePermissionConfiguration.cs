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

        // 配置审计字段（从 AuditableEntity 继承）
        builder.Property(ur => ur.CreatedAt)
            .IsRequired();

        builder.Property(ur => ur.CreatedBy)
            .HasMaxLength(50);

        builder.Property(ur => ur.UpdatedAt);

        builder.Property(ur => ur.UpdatedBy)
            .HasMaxLength(50);

        builder.Property(rp => rp.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(rp => rp.RoleId)
            .HasDatabaseName("ix_role_permissions_role_id");

        builder.HasIndex(rp => rp.PermissionCode)
            .HasDatabaseName("ix_role_permissions_permission_code");
    }
}
