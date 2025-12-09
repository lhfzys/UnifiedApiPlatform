using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public static class EntityConfigurationExtensions
{
    /// <summary>
    /// 配置行版本（乐观并发控制）
    /// </summary>
    public static void ConfigureRowVersion<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .HasColumnName("row_version")
            .ValueGeneratedOnAddOrUpdate();
    }
}
