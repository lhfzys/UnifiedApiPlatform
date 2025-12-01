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

        // 关系配置
        builder.HasOne(ar => ar.Announcement)
            .WithMany(a => a.ReadRecords)
            .HasForeignKey(ar => ar.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ar => ar.User)
            .WithMany()
            .HasForeignKey(ar => ar.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // 索引
        builder.HasIndex(ar => ar.ReadAt)
            .HasDatabaseName("ix_announcement_reads_read_at");
    }
}
