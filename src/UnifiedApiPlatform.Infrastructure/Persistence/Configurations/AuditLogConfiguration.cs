using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        // 主键
        builder.HasKey(a => a.Id);

        // TenantId
        builder.Property(a => a.TenantId)
            .HasMaxLength(50);

        // UserId
        builder.Property(a => a.UserId);

        // UserName
        builder.Property(a => a.UserName)
            .HasMaxLength(100);

        // Action
        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        // EntityType
        builder.Property(a => a.EntityType)
            .HasMaxLength(100);

        // EntityId
        builder.Property(a => a.EntityId)
            .HasMaxLength(50);

        // HttpMethod
        builder.Property(a => a.HttpMethod)
            .IsRequired()
            .HasMaxLength(10);

        // RequestPath
        builder.Property(a => a.RequestPath)
            .IsRequired()
            .HasMaxLength(500);

        // RequestBody（大文本，可能很长）
        builder.Property(a => a.RequestBody)
            .HasColumnType("text");

        // StatusCode
        builder.Property(a => a.StatusCode)
            .IsRequired();

        // ResponseBody
        builder.Property(a => a.ResponseBody)
            .HasColumnType("text");

        // Duration
        builder.Property(a => a.Duration)
            .IsRequired();

        // IpAddress
        builder.Property(a => a.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        // UserAgent
        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        // Exception
        builder.Property(a => a.Exception)
            .HasColumnType("text");

        // CreatedAt
        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // IsSuccess
        builder.Property(a => a.IsSuccess)
            .IsRequired();

        // RowVersion
        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        // 索引
        builder.HasIndex(a => a.TenantId)
            .HasDatabaseName("ix_audit_logs_tenant_id");

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("ix_audit_logs_user_id");

        builder.HasIndex(a => a.Action)
            .HasDatabaseName("ix_audit_logs_action");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("ix_audit_logs_created_at");

        builder.HasIndex(a => new { a.TenantId, a.CreatedAt })
            .HasDatabaseName("ix_audit_logs_tenant_created");

        // 关系配置
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
