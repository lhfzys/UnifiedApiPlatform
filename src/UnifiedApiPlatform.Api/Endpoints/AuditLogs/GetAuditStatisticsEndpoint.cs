using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetAuditStatistics;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.AuditLogs;

/// <summary>
/// 获取审计统计端点
/// </summary>
public class GetAuditStatisticsEndpoint : Endpoint<GetAuditStatisticsQuery>
{
    private readonly IMediator _mediator;

    public GetAuditStatisticsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("audit-logs/statistics");
        Permissions(PermissionCodes.AuditLogsView);
        Summary(s =>
        {
            s.Summary = "获取审计统计";
            s.Description = "获取审计日志统计信息，包括操作统计、用户活跃度、IP 统计等";
        });
    }

    public override async Task HandleAsync(GetAuditStatisticsQuery req, CancellationToken ct)
    {
        req.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(req, ct);

        await this.SendResultAsync(result, ct);
    }
}
