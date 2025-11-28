using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class ScheduledJobConfiguration : IEntityTypeConfiguration<ScheduledJob>
{
    public void Configure(EntityTypeBuilder<ScheduledJob> builder)
    {
        builder.ToTable("scheduled_jobs");

        builder.HasKey(sj => sj.Id);

        builder.Property(sj => sj.TenantId)
            .HasMaxLength(50);

        builder.Property(sj => sj.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sj => sj.JobType)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(sj => sj.CronExpression)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sj => sj.Parameters)
            .HasColumnType("jsonb");

        builder.Property(sj => sj.LastStatus)
            .HasMaxLength(50);

        builder.Property(sj => sj.Description)
            .HasMaxLength(500);

        builder.Property(sj => sj.CreatedBy)
            .IsRequired()
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(sj => sj.TenantId)
            .HasDatabaseName("ix_scheduled_jobs_tenant_id");

        builder.HasIndex(sj => sj.IsEnabled)
            .HasDatabaseName("ix_scheduled_jobs_enabled");

        builder.HasIndex(sj => sj.NextRunAt)
            .HasFilter("is_enabled = true")
            .HasDatabaseName("ix_scheduled_jobs_next_run");
    }
}
