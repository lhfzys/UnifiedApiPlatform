using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class RoleMenuConfiguration : IEntityTypeConfiguration<RoleMenu>
{
    public void Configure(EntityTypeBuilder<RoleMenu> builder)
    {
        builder.ToTable("role_menus");

        builder.HasKey(rm => new { rm.RoleId, rm.MenuId });

        // 配置审计字段（从 AuditableEntity 继承）
        builder.Property(ur => ur.CreatedAt)
            .IsRequired();

        builder.Property(ur => ur.CreatedBy)
            .HasMaxLength(50);

        builder.Property(ur => ur.UpdatedAt);

        builder.Property(ur => ur.UpdatedBy)
            .HasMaxLength(50);

        builder.Property(rm => rm.RowVersion).IsRowVersion();

        builder.HasIndex(rm => rm.RoleId)
            .HasDatabaseName("ix_role_menus_role_id");

        builder.HasIndex(rm => rm.MenuId)
            .HasDatabaseName("ix_role_menus_menu_id");

        builder.HasOne(rm => rm.Role)
            .WithMany(r => r.RoleMenus)
            .HasForeignKey(rm => rm.RoleId)
            .OnDelete(DeleteBehavior.Cascade);  // Role 删除时级联删除

        builder.HasOne(rm => rm.Menu)
            .WithMany(m => m.RoleMenus)
            .HasForeignKey(rm => rm.MenuId)
            .OnDelete(DeleteBehavior.Cascade);  // Menu 删除时级联删除
    }
}
