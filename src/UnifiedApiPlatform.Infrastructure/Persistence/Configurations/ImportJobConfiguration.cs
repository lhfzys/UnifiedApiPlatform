using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.ToTable("import_jobs");

        builder.HasKey(ij => ij.Id);

        builder.Property(ij => ij.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ij => ij.JobType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ij => ij.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ij => ij.ErrorMessage)
            .HasMaxLength(2000);

        // 索引
        builder.HasIndex(ij => ij.TenantId)
            .HasDatabaseName("ix_import_jobs_tenant_id");

        builder.HasIndex(ij => ij.UserId)
            .HasDatabaseName("ix_import_jobs_user_id");

        builder.HasIndex(ij => ij.Status)
            .HasDatabaseName("ix_import_jobs_status");

        builder.HasIndex(ij => ij.CreatedAt)
            .HasDatabaseName("ix_import_jobs_created_at");

        // 关系配置
        builder.HasMany(ij => ij.Details)
            .WithOne(ijd => ijd.Job)
            .HasForeignKey(ijd => ijd.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
