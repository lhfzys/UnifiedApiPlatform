using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetLoginLogs;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.AuditLogs;

/// <summary>
/// 查询登录日志端点
/// </summary>
public class GetLoginLogsEndpoint : Endpoint<GetLoginLogsQuery>
{
    private readonly IMediator _mediator;

    public GetLoginLogsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("audit-logs/login-logs");
        Permissions(PermissionCodes.AuditLogsView);
        Summary(s =>
        {
            s.Summary = "查询登录日志";
            s.Description = "分页查询登录日志，包括登录、登出、刷新令牌记录";
        });
    }

    public override async Task HandleAsync(GetLoginLogsQuery req, CancellationToken ct)
    {
        req.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(req, ct);

        await this.SendResultAsync(result, ct);
    }
}
