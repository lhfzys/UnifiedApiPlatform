using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class SystemSettingsConfiguration : IEntityTypeConfiguration<SystemSettings>
{
    public void Configure(EntityTypeBuilder<SystemSettings> builder)
    {
        builder.ToTable("system_settings");

        builder.HasKey(ss => ss.Id);

        builder.Property(ss => ss.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ss => ss.Value)
            .HasColumnType("text");

        builder.Property(ss => ss.DataType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ss => ss.Category)
            .HasMaxLength(100);

        builder.Property(ss => ss.Description)
            .HasMaxLength(500);

        // 唯一约束
        builder.HasIndex(ss => ss.Key)
            .IsUnique()
            .HasDatabaseName("ix_system_settings_key");

        // 索引
        builder.HasIndex(ss => ss.Category)
            .HasDatabaseName("ix_system_settings_category");
    }
}
