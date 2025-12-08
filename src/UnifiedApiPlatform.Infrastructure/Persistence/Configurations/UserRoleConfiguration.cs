using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // 配置审计字段（从 AuditableEntity 继承）
        builder.Property(ur => ur.CreatedAt)
            .IsRequired();

        builder.Property(ur => ur.CreatedBy)
            .HasMaxLength(50);

        builder.Property(ur => ur.UpdatedAt);

        builder.Property(ur => ur.UpdatedBy)
            .HasMaxLength(50);

        // 关系配置
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // 索引
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("ix_user_roles_user_id");

        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("ix_user_roles_role_id");
    }
}
