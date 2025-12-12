using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetAuditStatistics;

public class GetAuditStatisticsQueryHandler
    : IRequestHandler<GetAuditStatisticsQuery, Result<AuditStatisticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClock _clock;

    public GetAuditStatisticsQueryHandler(
        IApplicationDbContext context,
        IClock clock)
    {
        _context = context;
        _clock = clock;
    }

    public async Task<Result<AuditStatisticsDto>> Handle(
        GetAuditStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        // 默认最近 30 天
        var startTime = request.StartTime ?? DateTime.UtcNow.AddDays(-30);
        var endTime = request.EndTime ?? DateTime.UtcNow;

        var startInstant = Instant.FromDateTimeUtc(startTime.ToUniversalTime());
        var endInstant = Instant.FromDateTimeUtc(endTime.ToUniversalTime());

        // 基础查询
        var query = _context.AuditLogs
            .Where(a => a.CreatedAt >= startInstant && a.CreatedAt <= endInstant);

        // 总操作数
        var totalOperations = await query.CountAsync(cancellationToken);

        // 成功/失败操作数
        var successOperations = await query.CountAsync(a => a.IsSuccess, cancellationToken);
        var failedOperations = totalOperations - successOperations;

        // 平均响应时间
        var averageDuration = await query.AverageAsync(a => (double?)a.Duration, cancellationToken) ?? 0;

        // 按操作类型统计
        var actionStats = await query
            .GroupBy(a => a.Action)
            .Select(g => new ActionStatDto
            {
                Action = g.Key,
                Count = g.Count(),
                SuccessCount = g.Count(a => a.IsSuccess),
                FailedCount = g.Count(a => !a.IsSuccess)
            })
            .OrderByDescending(s => s.Count)
            .ToListAsync(cancellationToken);

        // Top 10 活跃用户
        var topUsers = await query
            .Where(a => a.UserName != null)
            .GroupBy(a => a.UserName)
            .Select(g => new UserActivityDto
            {
                UserName = g.Key!,
                OperationCount = g.Count()
            })
            .OrderByDescending(u => u.OperationCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        // Top 10 活跃 IP
        var topIps = await query
            .GroupBy(a => a.IpAddress)
            .Select(g => new IpActivityDto
            {
                IpAddress = g.Key,
                RequestCount = g.Count()
            })
            .OrderByDescending(i => i.RequestCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        // 按日期统计
        var dailyStats = await query
            .GroupBy(a => a.CreatedAt.InUtc().Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalCount = g.Count(),
                SuccessCount = g.Count(a => a.IsSuccess),
                FailedCount = g.Count(a => !a.IsSuccess)
            })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);

        var result = new AuditStatisticsDto
        {
            TotalOperations = totalOperations,
            SuccessOperations = successOperations,
            FailedOperations = failedOperations,
            AverageDuration = averageDuration,
            ActionStats = actionStats,
            TopUsers = topUsers,
            TopIps = topIps,
            DailyStats = dailyStats.Select(d => new DailyStatDto
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
