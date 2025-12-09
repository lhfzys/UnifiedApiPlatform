using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class LoginLogConfiguration : IEntityTypeConfiguration<LoginLog>
{
    public void Configure(EntityTypeBuilder<LoginLog> builder)
    {
        builder.ToTable("login_logs");

        // 主键
        builder.HasKey(l => l.Id);

        // TenantId
        builder.Property(l => l.TenantId)
            .HasMaxLength(50);

        // UserId
        builder.Property(l => l.UserId);

        // UserName
        builder.Property(l => l.UserName)
            .IsRequired()
            .HasMaxLength(100);

        // LoginType
        builder.Property(l => l.LoginType)
            .IsRequired()
            .HasMaxLength(50);

        // IsSuccess
        builder.Property(l => l.IsSuccess)
            .IsRequired();

        // FailureReason
        builder.Property(l => l.FailureReason)
            .HasMaxLength(500);

        // IpAddress
        builder.Property(l => l.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        // UserAgent
        builder.Property(l => l.UserAgent)
            .HasMaxLength(500);

        // Browser
        builder.Property(l => l.Browser)
            .HasMaxLength(100);

        // OperatingSystem
        builder.Property(l => l.OperatingSystem)
            .HasMaxLength(100);

        // DeviceType
        builder.Property(l => l.DeviceType)
            .HasMaxLength(50);

        // Location
        builder.Property(l => l.Location)
            .HasMaxLength(200);

        // CreatedAt
        builder.Property(l => l.CreatedAt)
            .IsRequired();

        // RowVersion
        builder.Property(l => l.RowVersion)
            .IsRowVersion();

        // 索引
        builder.HasIndex(l => l.TenantId)
            .HasDatabaseName("ix_login_logs_tenant_id");

        builder.HasIndex(l => l.UserId)
            .HasDatabaseName("ix_login_logs_user_id");

        builder.HasIndex(l => l.UserName)
            .HasDatabaseName("ix_login_logs_user_name");

        builder.HasIndex(l => l.CreatedAt)
            .HasDatabaseName("ix_login_logs_created_at");

        builder.HasIndex(l => new { l.TenantId, l.IsSuccess, l.CreatedAt })
            .HasDatabaseName("ix_login_logs_tenant_success_created");

        // 关系配置
        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
