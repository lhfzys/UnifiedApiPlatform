using FastEndpoints;
using Mapster;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Api.Endpoints;

/// <summary>
/// 查询端点基类（专门用于 QueryBase）
/// </summary>
public abstract class QueryEndpointBase<TRequest, TQuery, TResponse> : Endpoint<TRequest>
    where TQuery : QueryBase<TResponse>
{
    protected readonly IMediator Mediator;

    protected QueryEndpointBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        // 自动映射 Request -> Query
        var query = req.Adapt<TQuery>();

        // ✅ 在映射后注入租户上下文信息
        var currentUser = HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        if (currentUser.IsAuthenticated)
        {
            query.CurrentUserId = currentUser.UserId;
            query.CurrentTenantId = currentUser.TenantId;
        }

        query.TraceId = HttpContext.TraceIdentifier;

        // 发送查询并获取结果
        var result = await Mediator.Send(query, ct);

        // 自动发送响应
        await this.SendResultAsync(result, ct);
    }
}
