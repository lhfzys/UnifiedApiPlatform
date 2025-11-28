using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// 配置全局查询过滤器和忽略类型
    /// </summary>
    public static ModelBuilder ConfigureGlobalFilters(this ModelBuilder modelBuilder)
    {
        // 忽略不需要映射到数据库的类型
        modelBuilder.Ignore<DomainEvent>();

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // 转换表名为 snake_case
            var tableName = ToSnakeCase(entity.GetTableName() ?? entity.ClrType.Name);
            entity.SetTableName(tableName);

            // 转换列名为 snake_case
            foreach (var property in entity.GetProperties())
            {
                var columnName = ToSnakeCase(property.Name);
                property.SetColumnName(columnName);
            }

            // 转换索引名为 snake_case（如果有自定义名称）
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (keyName != null)
                {
                    key.SetName(ToSnakeCase(keyName));
                }
            }

            foreach (var key in entity.GetForeignKeys())
            {
                var constraintName = key.GetConstraintName();
                if (constraintName != null)
                {
                    key.SetConstraintName(ToSnakeCase(constraintName));
                }
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (indexName != null)
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }

        // 全局软删除过滤器
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
            {
                // 为所有继承 SoftDeletableEntity 的实体添加全局查询过滤器
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property =
                    System.Linq.Expressions.Expression.Property(parameter, nameof(SoftDeletableEntity.IsDeleted));
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(property,
                        System.Linq.Expressions.Expression.Constant(false)),
                    parameter);

                entityType.SetQueryFilter(filter);
            }
        }

        return modelBuilder;
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var startUnderscores = Regex.Match(input, @"^_+");
        return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}
