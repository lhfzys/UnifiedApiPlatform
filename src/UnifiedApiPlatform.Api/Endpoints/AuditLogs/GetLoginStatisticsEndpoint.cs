using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetLoginStatistics;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.AuditLogs;

/// <summary>
/// 获取登录统计端点
/// </summary>
public class GetLoginStatisticsEndpoint : Endpoint<GetLoginStatisticsQuery>
{
    private readonly IMediator _mediator;

    public GetLoginStatisticsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("audit-logs/login-statistics");
        Permissions(PermissionCodes.AuditLogsView);
        Summary(s =>
        {
            s.Summary = "获取登录统计";
            s.Description = "获取登录日志统计信息，包括成功率、地理位置分布、浏览器统计等";
        });
    }

    public override async Task HandleAsync(GetLoginStatisticsQuery req, CancellationToken ct)
    {
        req.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(req, ct);

        await this.SendResultAsync(result, ct);
    }
}
