using UnifiedApiPlatform.Application.Common.Commands;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Commands.ExportAuditLogs;

/// <summary>
/// 导出操作日志
/// </summary>
public class ExportAuditLogsCommand : CommandBase<ExportFileResult>
{
    /// <summary>
    /// 导出格式（excel/csv）
    /// </summary>
    public string Format { get; set; } = "excel";

    /// <summary>
    /// 用户名（模糊查询）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// 实体类型
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// HTTP 方法
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool? IsSuccess { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
