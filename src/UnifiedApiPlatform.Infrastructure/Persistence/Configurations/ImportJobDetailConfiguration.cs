using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class ImportJobDetailConfiguration : IEntityTypeConfiguration<ImportJobDetail>
{
    public void Configure(EntityTypeBuilder<ImportJobDetail> builder)
    {
        builder.ToTable("import_job_details");

        builder.HasKey(ijd => ijd.Id);

        builder.Property(ijd => ijd.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ijd => ijd.Data)
            .HasColumnType("jsonb");

        builder.Property(ijd => ijd.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(ijd => ijd.CreatedEntityId)
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(ijd => ijd.JobId)
            .HasDatabaseName("ix_import_job_details_job_id");

        builder.HasIndex(ijd => new { ijd.JobId, ijd.RowNumber })
            .HasDatabaseName("ix_import_job_details_job_row");

        builder.HasIndex(ijd => new { ijd.JobId, ijd.Status })
            .HasDatabaseName("ix_import_job_details_job_status");
    }
}
