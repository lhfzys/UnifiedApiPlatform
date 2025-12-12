using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetAuditStatistics;

/// <summary>
/// 获取审计日志统计
/// </summary>
public class GetAuditStatisticsQuery : QueryBase<AuditStatisticsDto>
{
    /// <summary>
    /// 开始时间（默认最近 30 天）
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间（默认现在）
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 审计统计 DTO
/// </summary>
public class AuditStatisticsDto
{
    /// <summary>
    /// 总操作数
    /// </summary>
    public long TotalOperations { get; set; }

    /// <summary>
    /// 成功操作数
    /// </summary>
    public long SuccessOperations { get; set; }

    /// <summary>
    /// 失败操作数
    /// </summary>
    public long FailedOperations { get; set; }

    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public double AverageDuration { get; set; }

    /// <summary>
    /// 按操作类型统计
    /// </summary>
    public List<ActionStatDto> ActionStats { get; set; } = new();

    /// <summary>
    /// 按用户统计（Top 10）
    /// </summary>
    public List<UserActivityDto> TopUsers { get; set; } = new();

    /// <summary>
    /// 按 IP 统计（Top 10）
    /// </summary>
    public List<IpActivityDto> TopIps { get; set; } = new();

    /// <summary>
    /// 按日期统计（最近 30 天）
    /// </summary>
    public List<DailyStatDto> DailyStats { get; set; } = new();
}

public class ActionStatDto
{
    public string Action { get; set; } = null!;
    public long Count { get; set; }
    public long SuccessCount { get; set; }
    public long FailedCount { get; set; }
}

public class UserActivityDto
{
    public string UserName { get; set; } = null!;
    public long OperationCount { get; set; }
}

public class IpActivityDto
{
    public string IpAddress { get; set; } = null!;
    public long RequestCount { get; set; }
}

public class DailyStatDto
{
    public DateTime Date { get; set; }
    public long TotalCount { get; set; }
    public long SuccessCount { get; set; }
    public long FailedCount { get; set; }
}
