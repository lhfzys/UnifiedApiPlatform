using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class UserNotificationSettingsConfiguration : IEntityTypeConfiguration<UserNotificationSettings>
{
    public void Configure(EntityTypeBuilder<UserNotificationSettings> builder)
    {
        builder.ToTable("user_notification_settings");

        builder.HasKey(uns => uns.Id);

        builder.Property(uns => uns.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(uns => uns.CategorySettings)
            .HasColumnType("jsonb");

        // 唯一约束
        builder.HasIndex(uns => uns.UserId)
            .IsUnique()
            .HasDatabaseName("ix_user_notification_settings_user_id");

        // 关系配置
        builder.HasOne(uns => uns.User)
            .WithMany()
            .HasForeignKey(uns => uns.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
