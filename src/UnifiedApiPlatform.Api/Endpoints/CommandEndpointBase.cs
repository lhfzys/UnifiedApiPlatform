using MediatR;
using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Api.Endpoints;

/// <summary>
/// 命令端点基类（专门用于 CommandBase）
/// </summary>
public abstract class CommandEndpointBase<TRequest, TCommand, TResponse> : EndpointBase<TRequest, TCommand, TResponse>
    where TCommand : CommandBase<TResponse>
{
    protected CommandEndpointBase(IMediator mediator) : base(mediator)
    {
    }
}

/// <summary>
/// 命令端点基类（无响应数据）
/// </summary>
public abstract class CommandEndpointBase<TRequest, TCommand> : EndpointBase<TRequest, TCommand>
    where TCommand : CommandBase
{
    protected CommandEndpointBase(IMediator mediator) : base(mediator)
    {
    }
}
