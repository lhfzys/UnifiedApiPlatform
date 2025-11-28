using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("announcements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId)
            .HasMaxLength(50);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(a => a.Priority)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.TargetAudience)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.TargetRoleIds)
            .HasColumnType("jsonb");

        builder.Property(a => a.TargetOrgIds)
            .HasColumnType("jsonb");

        // 索引
        builder.HasIndex(a => a.TenantId)
            .HasDatabaseName("ix_announcements_tenant_id");

        builder.HasIndex(a => a.PublisherId)
            .HasDatabaseName("ix_announcements_publisher_id");

        builder.HasIndex(a => a.PublishedAt)
            .HasDatabaseName("ix_announcements_published_at");

        builder.HasIndex(a => new { a.IsActive, a.ExpiresAt })
            .HasDatabaseName("ix_announcements_active_expires");

        // 关系配置
        builder.HasMany(a => a.ReadRecords)
            .WithOne(ar => ar.Announcement)
            .HasForeignKey(ar => ar.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
