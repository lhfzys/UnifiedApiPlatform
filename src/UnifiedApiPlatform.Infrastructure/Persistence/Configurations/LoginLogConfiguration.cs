using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class LoginLogConfiguration : IEntityTypeConfiguration<LoginLog>
{
    public void Configure(EntityTypeBuilder<LoginLog> builder)
    {
        builder.ToTable("login_logs");

        builder.HasKey(ll => ll.Id);

        builder.Property(ll => ll.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ll => ll.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ll => ll.LoginType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ll => ll.FailureReason)
            .HasMaxLength(500);

        builder.Property(ll => ll.IpAddress)
            .HasMaxLength(50);

        builder.Property(ll => ll.UserAgent)
            .HasMaxLength(500);

        builder.Property(ll => ll.DeviceType)
            .HasMaxLength(50);

        builder.Property(ll => ll.Location)
            .HasMaxLength(200);

        // 索引
        builder.HasIndex(ll => ll.TenantId)
            .HasDatabaseName("ix_login_logs_tenant_id");

        builder.HasIndex(ll => ll.UserId)
            .HasDatabaseName("ix_login_logs_user_id");

        builder.HasIndex(ll => ll.UserName)
            .HasDatabaseName("ix_login_logs_username");

        builder.HasIndex(ll => ll.LoginAt)
            .HasDatabaseName("ix_login_logs_login_at");

        builder.HasIndex(ll => ll.Status)
            .HasDatabaseName("ix_login_logs_status");

        builder.HasIndex(ll => new { ll.UserId, ll.Status })
            .HasDatabaseName("ix_login_logs_user_status");

        // 关系配置
        builder.HasOne(ll => ll.User)
            .WithMany()
            .HasForeignKey(ll => ll.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
