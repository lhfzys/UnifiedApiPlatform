using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
{
    public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        builder.ToTable("notification_recipients");

        builder.HasKey(nr => new { nr.NotificationId, nr.UserId });

        // 索引
        builder.HasIndex(nr => nr.UserId)
            .HasDatabaseName("ix_notification_recipients_user_id");

        builder.HasIndex(nr => new { nr.UserId, nr.IsRead })
            .HasDatabaseName("ix_notification_recipients_user_read");

        builder.HasIndex(nr => nr.NotificationId)
            .HasDatabaseName("ix_notification_recipients_notification_id");
    }
}
