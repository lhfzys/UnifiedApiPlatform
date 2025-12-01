using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class ImportJobDetailConfiguration : IEntityTypeConfiguration<ImportJobDetail>
{
    public void Configure(EntityTypeBuilder<ImportJobDetail> builder)
    {
        builder.ToTable("import_job_details");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Data)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(d => d.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(d => d.CreatedEntityId)
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(d => d.JobId)
            .HasDatabaseName("ix_import_job_details_job_id");

        builder.HasIndex(d => new { d.JobId, d.RowNumber })
            .HasDatabaseName("ix_import_job_details_job_row");

        builder.HasIndex(d => new { d.JobId, d.Status })
            .HasDatabaseName("ix_import_job_details_job_status");

        // 关系配置
        builder.HasOne(d => d.Job)
            .WithMany(j => j.Details)
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
