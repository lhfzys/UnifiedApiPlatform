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

        builder.HasIndex(rm => rm.RoleId)
            .HasDatabaseName("ix_role_menus_role_id");

        builder.HasIndex(rm => rm.MenuId)
            .HasDatabaseName("ix_role_menus_menu_id");
    }
}
