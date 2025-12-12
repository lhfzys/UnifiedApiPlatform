using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using UnifiedApiPlatform.Application.Features.AuditLogs.Commands.ExportAuditLogs;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.AuditLogs;

/// <summary>
/// 导出操作日志端点
/// </summary>
public class ExportAuditLogsEndpoint : Endpoint<ExportAuditLogsCommand>
{
    private readonly IMediator _mediator;

    public ExportAuditLogsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("audit-logs/export");
        Permissions(PermissionCodes.AuditLogsExport);
        Summary(s =>
        {
            s.Summary = "导出操作日志";
            s.Description = "导出操作日志为 Excel 或 CSV 文件，支持筛选条件";
        });
    }

    public override async Task HandleAsync(ExportAuditLogsCommand req, CancellationToken ct)
    {
        req.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(req, ct);

        if (result.IsFailed)
        {
            ThrowError(result.Errors.FirstOrDefault()?.Message ?? "导出失败");
            return;
        }

        var fileResult = result.Value;

        // 返回文件
        await Send.BytesAsync(
            fileResult.FileContent,
            fileName: fileResult.FileName,
            contentType: fileResult.ContentType,
            cancellation: ct);
    }
}
