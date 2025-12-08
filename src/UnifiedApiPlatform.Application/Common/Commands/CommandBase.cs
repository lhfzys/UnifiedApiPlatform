using FluentResults;
using MediatR;

namespace UnifiedApiPlatform.Application.Common.Commands;

/// <summary>
/// 命令基类（无返回值）
/// </summary>
public abstract class CommandBase : IRequest<Result>
{
    /// <summary>
    /// 客户端 IP 地址（由预处理器注入）
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 用户代理（由预处理器注入）
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 追踪 ID（由预处理器注入）
    /// </summary>
    public string? TraceId { get; set; }
}

/// <summary>
/// 命令基类（带返回值）
/// </summary>
public abstract class CommandBase<TResponse> : IRequest<Result<TResponse>>
{
    /// <summary>
    /// 客户端 IP 地址（由预处理器注入）
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 用户代理（由预处理器注入）
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 追踪 ID（由预处理器注入）
    /// </summary>
    public string? TraceId { get; set; }
}
