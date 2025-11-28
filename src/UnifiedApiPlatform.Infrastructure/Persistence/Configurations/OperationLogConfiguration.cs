using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.ToTable("operation_logs");

        builder.HasKey(ol => ol.Id);

        builder.Property(ol => ol.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ol => ol.UserId)
            .HasMaxLength(50);

        builder.Property(ol => ol.UserName)
            .HasMaxLength(100);

        builder.Property(ol => ol.RequestPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ol => ol.RequestMethod)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(ol => ol.RequestHeaders)
            .HasColumnType("jsonb");

        builder.Property(ol => ol.RequestBody)
            .HasColumnType("text");

        builder.Property(ol => ol.RequestQuery)
            .HasMaxLength(2000);

        builder.Property(ol => ol.ResponseBody)
            .HasColumnType("text");

        builder.Property(ol => ol.IpAddress)
            .HasMaxLength(50);

        builder.Property(ol => ol.UserAgent)
            .HasMaxLength(500);

        builder.Property(ol => ol.TraceId)
            .HasMaxLength(100);

        builder.Property(ol => ol.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(ol => ol.StackTrace)
            .HasColumnType("text");

        // 索引
        builder.HasIndex(ol => ol.TenantId)
            .HasDatabaseName("ix_operation_logs_tenant_id");

        builder.HasIndex(ol => ol.UserId)
            .HasDatabaseName("ix_operation_logs_user_id");

        builder.HasIndex(ol => ol.Timestamp)
            .HasDatabaseName("ix_operation_logs_timestamp");

        builder.HasIndex(ol => ol.ResponseStatus)
            .HasDatabaseName("ix_operation_logs_status");

        builder.HasIndex(ol => ol.TraceId)
            .HasDatabaseName("ix_operation_logs_trace_id");
    }
}
