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

        builder.HasIndex(rp => rp.RoleId)
            .HasDatabaseName("ix_role_permissions_role_id");

        builder.HasIndex(rp => rp.PermissionCode)
            .HasDatabaseName("ix_role_permissions_permission_code");
    }
}
