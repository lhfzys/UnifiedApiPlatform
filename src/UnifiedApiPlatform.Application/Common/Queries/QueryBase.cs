using FluentResults;
using MediatR;

namespace UnifiedApiPlatform.Application.Common.Queries;

/// <summary>
/// 查询基类
/// </summary>
public abstract class QueryBase<TResponse> : IRequest<Result<TResponse>>, IQueryRequest
{
    /// <summary>
    /// 当前用户 ID（由预处理器注入）
    /// </summary>
    public string? CurrentUserId { get; set; }

    /// <summary>
    /// 当前租户 ID（由预处理器注入）
    /// </summary>
    public string? CurrentTenantId { get; set; }

    /// <summary>
    /// 追踪 ID（由预处理器注入）
    /// </summary>
    public string? TraceId { get; set; }
}
