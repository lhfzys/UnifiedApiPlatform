using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class DictionaryCategoryConfiguration : IEntityTypeConfiguration<DictionaryCategory>
{
    public void Configure(EntityTypeBuilder<DictionaryCategory> builder)
    {
        builder.ToTable("dictionary_categories");

        builder.HasKey(dc => dc.Id);

        builder.Property(dc => dc.TenantId)
            .HasMaxLength(50);

        builder.Property(dc => dc.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(dc => dc.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(dc => dc.Description)
            .HasMaxLength(500);

        // 唯一约束：Code 全局或租户内唯一
        builder.HasIndex(dc => new { dc.TenantId, dc.Code })
            .IsUnique()
            .HasDatabaseName("ix_dictionary_categories_tenant_code");

        // 索引
        builder.HasIndex(dc => dc.TenantId)
            .HasDatabaseName("ix_dictionary_categories_tenant_id");

        // 关系配置
        builder.HasMany(dc => dc.Items)
            .WithOne(di => di.Category)
            .HasForeignKey(di => di.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
