using System.Linq.Expressions;

namespace UnifiedApiPlatform.Shared.Extensions;

/// <summary>
/// IQueryable 扩展方法
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// 条件过滤
    /// </summary>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// 分页
    /// </summary>
    public static IQueryable<T> PageBy<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// 动态排序
    /// </summary>
    public static IQueryable<T> OrderByDynamic<T>(
        this IQueryable<T> query,
        string? propertyName,
        bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = typeof(T).GetProperty(propertyName);

        if (property == null)
            return query;

        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var methodName = descending ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(orderByExpression));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}
