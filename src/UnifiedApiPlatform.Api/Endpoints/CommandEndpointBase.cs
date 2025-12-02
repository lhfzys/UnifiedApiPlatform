using FastEndpoints;
using Mapster;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Api.Endpoints;

/// <summary>
/// 命令端点基类（专门用于 CommandBase）
/// </summary>
public abstract class CommandEndpointBase<TRequest, TCommand, TResponse> : Endpoint<TRequest>
    where TCommand : CommandBase<TResponse>
{
    protected readonly IMediator Mediator;

    protected CommandEndpointBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        // 自动映射 Request -> Command
        var command = req.Adapt<TCommand>();

        // ✅ 在映射后注入 HTTP 上下文信息
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        command.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        command.TraceId = HttpContext.TraceIdentifier;

        // 发送命令并获取结果
        var result = await Mediator.Send(command, ct);

        // 自动发送响应
        await this.SendResultAsync(result, ct);
    }
}

/// <summary>
/// 命令端点基类（无响应数据）
/// </summary>
public abstract class CommandEndpointBase<TRequest, TCommand> : Endpoint<TRequest>
    where TCommand : CommandBase
{
    protected readonly IMediator Mediator;

    protected CommandEndpointBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        // 自动映射 Request -> Command
        var command = req.Adapt<TCommand>();

        // ✅ 在映射后注入 HTTP 上下文信息
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        command.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        command.TraceId = HttpContext.TraceIdentifier;

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
