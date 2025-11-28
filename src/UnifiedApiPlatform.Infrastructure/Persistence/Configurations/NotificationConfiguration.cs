using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.Category)
            .HasMaxLength(50);

        builder.Property(n => n.SenderName)
            .HasMaxLength(100);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        builder.Property(n => n.ActionText)
            .HasMaxLength(50);

        builder.Property(n => n.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(n => n.RelatedEntityId)
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(n => n.TenantId)
            .HasDatabaseName("ix_notifications_tenant_id");

        builder.HasIndex(n => n.SenderId)
            .HasDatabaseName("ix_notifications_sender_id");

        builder.HasIndex(n => n.CreatedAt)
            .HasDatabaseName("ix_notifications_created_at");

        builder.HasIndex(n => n.Type)
            .HasDatabaseName("ix_notifications_type");

        // 关系配置
        builder.HasMany(n => n.Recipients)
            .WithOne(nr => nr.Notification)
            .HasForeignKey(nr => nr.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
