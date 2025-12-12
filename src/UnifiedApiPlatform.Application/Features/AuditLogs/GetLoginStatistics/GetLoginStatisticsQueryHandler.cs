using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetLoginStatistics;

public class GetLoginStatisticsQueryHandler
    : IRequestHandler<GetLoginStatisticsQuery, Result<LoginStatisticsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLoginStatisticsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LoginStatisticsDto>> Handle(
        GetLoginStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        // 默认最近 30 天
        var startTime = request.StartTime ?? DateTime.UtcNow.AddDays(-30);
        var endTime = request.EndTime ?? DateTime.UtcNow;

        var startInstant = Instant.FromDateTimeUtc(startTime.ToUniversalTime());
        var endInstant = Instant.FromDateTimeUtc(endTime.ToUniversalTime());

        // 基础查询
        var query = _context.LoginLogs
            .Where(l => l.CreatedAt >= startInstant && l.CreatedAt <= endInstant);

        // 总登录次数
        var totalLogins = await query.CountAsync(cancellationToken);

        // 成功/失败登录次数
        var successLogins = await query.CountAsync(l => l.IsSuccess, cancellationToken);
        var failedLogins = totalLogins - successLogins;

        // 登录成功率
        var successRate = totalLogins > 0 ? (double)successLogins / totalLogins * 100 : 0;

        // 按登录类型统计
        var loginTypeStats = await query
            .GroupBy(l => l.LoginType)
            .Select(g => new LoginTypeStatDto
            {
                LoginType = g.Key.ToString(),
                Count = g.Count(),
                SuccessCount = g.Count(l => l.IsSuccess),
                FailedCount = g.Count(l => !l.IsSuccess)
            })
            .ToListAsync(cancellationToken);

        // 按地理位置统计（Top 10）
        var topLocations = await query
            .Where(l => l.Location != null)
            .GroupBy(l => l.Location)
            .Select(g => new LocationStatDto
            {
                Location = g.Key!,
                LoginCount = g.Count()
            })
            .OrderByDescending(l => l.LoginCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        // 按浏览器统计
        var browserStats = await query
            .Where(l => l.Browser != null)
            .GroupBy(l => l.Browser)
            .Select(g => new BrowserStatDto
            {
                Browser = g.Key!,
                Count = g.Count()
            })
            .OrderByDescending(b => b.Count)
            .ToListAsync(cancellationToken);

        // 按操作系统统计
        var osStats = await query
            .Where(l => l.OperatingSystem != null)
            .GroupBy(l => l.OperatingSystem)
            .Select(g => new OsStatDto
            {
                OperatingSystem = g.Key!,
                Count = g.Count()
            })
            .OrderByDescending(o => o.Count)
            .ToListAsync(cancellationToken);

        // 失败原因统计
        var failureReasons = await query
            .Where(l => !l.IsSuccess && l.FailureReason != null)
            .GroupBy(l => l.FailureReason)
            .Select(g => new FailureReasonStatDto
            {
                Reason = g.Key!,
                Count = g.Count()
            })
            .OrderByDescending(f => f.Count)
            .ToListAsync(cancellationToken);

        // 按日期统计
        var dailyStats = await query
            .GroupBy(l => l.CreatedAt.InUtc().Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalCount = g.Count(),
                SuccessCount = g.Count(l => l.IsSuccess),
                FailedCount = g.Count(l => !l.IsSuccess)
            })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);

        var result = new LoginStatisticsDto
        {
            TotalLogins = totalLogins,
            SuccessLogins = successLogins,
            FailedLogins = failedLogins,
            SuccessRate = Math.Round(successRate, 2),
            LoginTypeStats = loginTypeStats,
            TopLocations = topLocations,
            BrowserStats = browserStats,
            OsStats = osStats,
            FailureReasons = failureReasons,
            DailyStats = dailyStats.Select(d => new DailyLoginStatDto
            {
                Date = d.Date.ToDateTimeUnspecified(),
                TotalCount = d.TotalCount,
                SuccessCount = d.SuccessCount,
                FailedCount = d.FailedCount
            }).ToList()
        };

        return Result.Ok(result);
    }
}
