using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.ToTable("tenant_settings");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ts => ts.Value)
            .HasColumnType("text");

        builder.Property(ts => ts.DataType)
            .IsRequired()
            .HasMaxLength(50);

        // 唯一约束
        builder.HasIndex(ts => new { ts.TenantId, ts.Key })
            .IsUnique()
            .HasDatabaseName("ix_tenant_settings_tenant_key");

        // 索引
        builder.HasIndex(ts => ts.TenantId)
            .HasDatabaseName("ix_tenant_settings_tenant_id");
    }
}
