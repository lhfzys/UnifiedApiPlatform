using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        builder.Property(u => u.LastLoginIp)
            .HasMaxLength(50);

        builder.Property(u => u.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LockedUntil)
            .HasColumnName("locked_until");

        builder.ConfigureRowVersion();

        // 唯一约束：租户内邮箱唯一
        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique()
            .HasFilter("is_deleted = false")
            .HasDatabaseName("ix_users_tenant_email");

        // 唯一约束：租户内用户名唯一
        builder.HasIndex(u => new { u.TenantId, u.UserName })
            .IsUnique()
            .HasFilter("is_deleted = false")
            .HasDatabaseName("ix_users_tenant_username");

        // 索引
        builder.HasIndex(u => u.TenantId)
            .HasDatabaseName("ix_users_tenant_id");

        builder.HasIndex(u => u.OrganizationId)
            .HasDatabaseName("ix_users_organization_id");

        // 关系配置
        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .HasPrincipalKey(t => t.Identifier)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Manager)
            .WithMany()
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 忽略领域事件（不映射到数据库）
        builder.Ignore(u => u.DomainEvents);
    }
}
