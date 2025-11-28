using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.UserId)
            .HasMaxLength(50);

        builder.Property(al => al.UserName)
            .HasMaxLength(100);

        builder.Property(al => al.IpAddress)
            .HasMaxLength(50);

        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        builder.Property(al => al.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.EntityId)
            .HasMaxLength(50);

        builder.Property(al => al.EntityName)
            .HasMaxLength(200);

        builder.Property(al => al.OldValues)
            .HasColumnType("jsonb");

        builder.Property(al => al.NewValues)
            .HasColumnType("jsonb");

        builder.Property(al => al.ChangedProperties)
            .HasMaxLength(1000);

        builder.Property(al => al.RequestPath)
            .HasMaxLength(500);

        builder.Property(al => al.RequestMethod)
            .HasMaxLength(10);

        builder.Property(al => al.RequestBody)
            .HasColumnType("text");

        builder.Property(al => al.TraceId)
            .HasMaxLength(100);

        builder.Property(al => al.ErrorMessage)
            .HasMaxLength(2000);

        // 索引
        builder.HasIndex(al => al.TenantId)
            .HasDatabaseName("ix_audit_logs_tenant_id");

        builder.HasIndex(al => al.UserId)
            .HasDatabaseName("ix_audit_logs_user_id");

        builder.HasIndex(al => new { al.EntityType, al.EntityId })
            .HasDatabaseName("ix_audit_logs_entity");

        builder.HasIndex(al => al.Timestamp)
            .HasDatabaseName("ix_audit_logs_timestamp");

        builder.HasIndex(al => al.Action)
            .HasDatabaseName("ix_audit_logs_action");
    }
}
