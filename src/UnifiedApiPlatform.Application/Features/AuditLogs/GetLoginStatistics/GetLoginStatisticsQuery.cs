using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetLoginStatistics;

/// <summary>
/// 获取登录统计
/// </summary>
public class GetLoginStatisticsQuery : QueryBase<LoginStatisticsDto>
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
/// 登录统计 DTO
/// </summary>
public class LoginStatisticsDto
{
    /// <summary>
    /// 总登录次数
    /// </summary>
    public long TotalLogins { get; set; }

    /// <summary>
    /// 成功登录次数
    /// </summary>
    public long SuccessLogins { get; set; }

    /// <summary>
    /// 失败登录次数
    /// </summary>
    public long FailedLogins { get; set; }

    /// <summary>
    /// 登录成功率
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// 按登录类型统计
    /// </summary>
    public List<LoginTypeStatDto> LoginTypeStats { get; set; } = new();

    /// <summary>
    /// 按地理位置统计（Top 10）
    /// </summary>
    public List<LocationStatDto> TopLocations { get; set; } = new();

    /// <summary>
    /// 按浏览器统计
    /// </summary>
    public List<BrowserStatDto> BrowserStats { get; set; } = new();

    /// <summary>
    /// 按操作系统统计
    /// </summary>
    public List<OsStatDto> OsStats { get; set; } = new();

    /// <summary>
    /// 失败原因统计
    /// </summary>
    public List<FailureReasonStatDto> FailureReasons { get; set; } = new();

    /// <summary>
    /// 按日期统计
    /// </summary>
    public List<DailyLoginStatDto> DailyStats { get; set; } = new();
}

public class LoginTypeStatDto
{
    public string LoginType { get; set; } = null!;
    public long Count { get; set; }
    public long SuccessCount { get; set; }
    public long FailedCount { get; set; }
}

public class LocationStatDto
{
    public string Location { get; set; } = null!;
    public long LoginCount { get; set; }
}

public class BrowserStatDto
{
    public string Browser { get; set; } = null!;
    public long Count { get; set; }
}

public class OsStatDto
{
    public string OperatingSystem { get; set; } = null!;
    public long Count { get; set; }
}

public class FailureReasonStatDto
{
    public string Reason { get; set; } = null!;
    public long Count { get; set; }
}

public class DailyLoginStatDto
{
    public DateTime Date { get; set; }
    public long TotalCount { get; set; }
    public long SuccessCount { get; set; }
    public long FailedCount { get; set; }
}
