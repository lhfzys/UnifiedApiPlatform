using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.PermissionCode)
            .HasMaxLength(100);

        builder.Property(m => m.Icon)
            .HasMaxLength(100);

        builder.Property(m => m.Path)
            .HasMaxLength(500);

        builder.Property(m => m.Component)
            .HasMaxLength(500);

        builder.Property(m => m.TenantId)
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(m => m.ParentId)
            .HasDatabaseName("ix_menus_parent_id");

        builder.HasIndex(m => new { m.TenantId, m.Name })
            .HasDatabaseName("ix_menus_tenant_name");

        // 自引用关系
        builder.HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // 关系配置
        builder.HasMany(m => m.RoleMenus)
            .WithOne(rm => rm.Menu)
            .HasForeignKey(rm => rm.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
