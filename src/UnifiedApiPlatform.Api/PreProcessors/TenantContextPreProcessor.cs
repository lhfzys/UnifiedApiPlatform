using FastEndpoints;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Api.PreProcessors;

/// <summary>
/// 租户上下文信息注入预处理器
/// 自动注入 CurrentUserId、CurrentTenantId
/// </summary>
public class TenantContextPreProcessor<TRequest> : IPreProcessor<TRequest>
{
    public Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
    {
        if (ctx.Request is IQueryRequest query)
        {
            // 从请求作用域解析 ICurrentUserService
            var currentUser = ctx.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
            // 如果用户已认证，注入用户信息
            if (currentUser.IsAuthenticated)
            {
                query.CurrentUserId = currentUser.UserId;
                query.CurrentTenantId = currentUser.TenantId;
            }

            // 注入追踪 ID
            query.TraceId = ctx.HttpContext.TraceIdentifier;
        }

        return Task.CompletedTask;
    }
}
