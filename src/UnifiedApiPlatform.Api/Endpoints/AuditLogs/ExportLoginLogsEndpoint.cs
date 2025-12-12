using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Application.Features.AuditLogs.Commands.ExportLoginLogs;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.AuditLogs;

/// <summary>
/// 导出登录日志端点
/// </summary>
public class ExportLoginLogsEndpoint : Endpoint<ExportLoginLogsCommand>
{
    private readonly IMediator _mediator;

    public ExportLoginLogsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("audit-logs/login-logs/export");
        Permissions(PermissionCodes.AuditLogsExport);
        Summary(s =>
        {
            s.Summary = "导出登录日志";
            s.Description = "导出登录日志为 Excel 或 CSV 文件，支持筛选条件";
        });
    }

    public override async Task HandleAsync(ExportLoginLogsCommand req, CancellationToken ct)
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
