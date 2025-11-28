using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class FileRecordConfiguration : IEntityTypeConfiguration<FileRecord>
{
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.ToTable("file_records");

        builder.HasKey(fr => fr.Id);

        builder.Property(fr => fr.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(fr => fr.StorageKey)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(fr => fr.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(fr => fr.FileHash)
            .HasMaxLength(100);

        builder.Property(fr => fr.UploadedBy)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(fr => fr.Metadata)
            .HasColumnType("jsonb");

        builder.Property(fr => fr.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(fr => fr.TenantId)
            .HasDatabaseName("ix_file_records_tenant_id");

        builder.HasIndex(fr => fr.FileHash)
            .HasDatabaseName("ix_file_records_hash");

        builder.HasIndex(fr => fr.UploadedBy)
            .HasDatabaseName("ix_file_records_uploaded_by");

        builder.HasIndex(fr => fr.UploadedAt)
            .HasDatabaseName("ix_file_records_uploaded_at");

        builder.HasIndex(fr => fr.Category)
            .HasDatabaseName("ix_file_records_category");

        builder.HasIndex(fr => fr.ExpiresAt)
            .HasFilter("expires_at IS NOT NULL")
            .HasDatabaseName("ix_file_records_expires_at");

        // 关系配置
        builder.HasMany(fr => fr.EntityAttachments)
            .WithOne(ea => ea.File)
            .HasForeignKey(ea => ea.FileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
