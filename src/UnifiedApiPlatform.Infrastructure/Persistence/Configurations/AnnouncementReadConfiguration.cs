using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class AnnouncementReadConfiguration : IEntityTypeConfiguration<AnnouncementRead>
{
    public void Configure(EntityTypeBuilder<AnnouncementRead> builder)
    {
        builder.ToTable("announcement_reads");

        builder.HasKey(ar => new { ar.AnnouncementId, ar.UserId });

        // 索引
        builder.HasIndex(ar => ar.UserId)
            .HasDatabaseName("ix_announcement_reads_user_id");

        builder.HasIndex(ar => ar.AnnouncementId)
            .HasDatabaseName("ix_announcement_reads_announcement_id");

        builder.HasIndex(ar => ar.ReadAt)
            .HasDatabaseName("ix_announcement_reads_read_at");
    }
}
