using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetAuditLogs;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.AuditLogs;

/// <summary>
/// 查询操作日志端点
/// </summary>
public class GetAuditLogsEndpoint : Endpoint<GetAuditLogsQuery>
{
    private readonly IMediator _mediator;

    public GetAuditLogsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("audit-logs");
        Permissions(PermissionCodes.AuditLogsView);
        Summary(s =>
        {
            s.Summary = "查询操作日志";
            s.Description = "分页查询操作审计日志，支持多条件筛选和排序";
        });
    }

    public override async Task HandleAsync(GetAuditLogsQuery req, CancellationToken ct)
    {
        // 注入上下文信息
        req.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(req, ct);

        await this.SendResultAsync(result, ct);
    }
}
