using UnifiedApiPlatform.Application.Common.Commands;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Commands.ExportLoginLogs;

/// <summary>
/// 导出登录日志
/// </summary>
public class ExportLoginLogsCommand : CommandBase<ExportFileResult>
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
    /// 登录类型
    /// </summary>
    public string? LoginType { get; set; }

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
