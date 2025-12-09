using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        // 主键
        builder.HasKey(rt => rt.Id);

        // UserId
        builder.Property(rt => rt.UserId)
            .IsRequired();

        // TenantId
        builder.Property(rt => rt.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        // Token
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        // ExpiresAt
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        // CreatedAt
        builder.Property(rt => rt.CreatedAt)
            .IsRequired();

        // CreatedByIp
        builder.Property(rt => rt.CreatedByIp)
            .IsRequired()
            .HasMaxLength(50);

        // RevokedAt
        builder.Property(rt => rt.RevokedAt);

        // RevokedByIp
        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(50);

        // ReplacedByToken
        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        // RevokeReason
        builder.Property(rt => rt.RevokeReason)
            .HasMaxLength(200);

        // DeviceInfo
        builder.Property(rt => rt.DeviceInfo)
            .HasMaxLength(500);


        builder.Property(rt => rt.RowVersion).IsRowVersion();
        // 索引
        builder.HasIndex(rt => rt.Token)
            .HasDatabaseName("ix_refresh_tokens_token")
            .IsUnique();

        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("ix_refresh_tokens_user_id");

        builder.HasIndex(rt => new { rt.UserId, rt.ExpiresAt })
            .HasDatabaseName("ix_refresh_tokens_user_expires");

        // 关系配置
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
