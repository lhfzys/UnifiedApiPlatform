using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public static class EntityConfigurationExtensions
{
    /// <summary>
    /// 配置行版本（并发令牌）
    /// </summary>
    public static PropertyBuilder<byte[]?> ConfigureRowVersion<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : MultiTenantEntity
    {
        return builder.Property(e => e.RowVersion)
            .IsRowVersion();
        ;
    }
}
