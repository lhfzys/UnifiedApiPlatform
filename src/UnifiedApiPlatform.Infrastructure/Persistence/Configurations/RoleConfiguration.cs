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

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.ConfigureRowVersion();

        // 唯一约束：租户内角色名唯一
        builder.HasIndex(r => new { r.TenantId, r.Name })
            .IsUnique()
            .HasFilter("is_deleted = false")
            .HasDatabaseName("ix_roles_tenant_name");

        // 索引
        builder.HasIndex(r => r.TenantId)
            .HasDatabaseName("ix_roles_tenant_id");

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

        builder.HasMany(r => r.DataScopes)
            .WithOne(ds => ds.Role)
            .HasForeignKey(ds => ds.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.DomainEvents);
    }
}
