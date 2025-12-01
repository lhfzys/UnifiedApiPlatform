using MediatR;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Api.Endpoints;

/// <summary>
/// 查询端点基类（专门用于 QueryBase）
/// </summary>
public abstract class QueryEndpointBase<TRequest, TQuery, TResponse> : EndpointBase<TRequest, TQuery, TResponse>
    where TQuery : QueryBase<TResponse>
{
    protected QueryEndpointBase(IMediator mediator) : base(mediator)
    {
    }
}
