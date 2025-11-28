using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class DictionaryItemConfiguration : IEntityTypeConfiguration<DictionaryItem>
{
    public void Configure(EntityTypeBuilder<DictionaryItem> builder)
    {
        builder.ToTable("dictionary_items");

        builder.HasKey(di => di.Id);

        builder.Property(di => di.TenantId)
            .HasMaxLength(50);

        builder.Property(di => di.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(di => di.Label)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(di => di.Value)
            .HasMaxLength(500);

        builder.Property(di => di.Color)
            .HasMaxLength(50);

        builder.Property(di => di.Icon)
            .HasMaxLength(100);

        builder.Property(di => di.ExtraData)
            .HasColumnType("jsonb");

        // 索引
        builder.HasIndex(di => di.CategoryId)
            .HasDatabaseName("ix_dictionary_items_category_id");

        builder.HasIndex(di => new { di.CategoryId, di.Code })
            .HasDatabaseName("ix_dictionary_items_category_code");

        builder.HasIndex(di => di.ParentId)
            .HasDatabaseName("ix_dictionary_items_parent_id");

        builder.HasIndex(di => new { di.CategoryId, di.Sort })
            .HasDatabaseName("ix_dictionary_items_category_sort");
    }
}
