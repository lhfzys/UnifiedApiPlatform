using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Common.Queries;

/// <summary>
/// 分页查询基类
/// </summary>
public abstract class PagedQueryBase<TResponse> : QueryBase<PagedResult<TResponse>>
{
    /// <summary>
    /// 页码（从 1 开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool SortDescending { get; set; } = false;

    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string? SearchKeyword { get; set; }
}
