using FastEndpoints;
using FluentResults;
using Mapster;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Api.Endpoints;

/// <summary>
/// 标准端点基类（带请求和响应）
/// </summary>
public abstract class EndpointBase<TRequest, TCommand, TResponse> : Endpoint<TRequest>
    where TCommand : IRequest<Result<TResponse>>
{
    protected readonly IMediator Mediator;

    protected EndpointBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        // 自动映射 Request -> Command
        var command = req.Adapt<TCommand>();

        // 发送命令并获取结果
        var result = await Mediator.Send(command, ct);

        // 自动发送响应
        await this.SendResultAsync(result, ct);
    }
}

/// <summary>
/// 无响应数据的端点基类
/// </summary>
public abstract class EndpointBase<TRequest, TCommand> : Endpoint<TRequest>
    where TCommand : IRequest<Result>
{
    protected readonly IMediator Mediator;

    protected EndpointBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        // 自动映射 Request -> Command
        var command = req.Adapt<TCommand>();

        // 发送命令并获取结果
        var result = await Mediator.Send(command, ct);

        // 自动发送响应
        if (result.IsSuccess)
        {
            await this.SendOkAsync(ct: ct);
        }
        else
        {
            var firstError = result.Errors.First();
            await this.SendErrorAsync(firstError.Message, ct: ct);
        }
    }
}
