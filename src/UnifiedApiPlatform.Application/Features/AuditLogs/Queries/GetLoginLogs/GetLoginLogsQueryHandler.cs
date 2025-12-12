using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetLoginLogs;

public class GetLoginLogsQueryHandler : IRequestHandler<GetLoginLogsQuery, Result<PagedResult<LoginLogDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLoginLogsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<LoginLogDto>>> Handle(
        GetLoginLogsQuery request,
        CancellationToken cancellationToken)
    {
        // 构建查询
        var query = _context.LoginLogs.AsQueryable();

        // 筛选条件
        if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(l => l.UserName != null && l.UserName.Contains(request.UserName));
        }

        if (!string.IsNullOrEmpty(request.LoginType))
        {
            query = query.Where(l => l.LoginType == request.LoginType);
        }

        if (request.IsSuccess.HasValue)
        {
            query = query.Where(l => l.IsSuccess == request.IsSuccess.Value);
        }

        if (!string.IsNullOrEmpty(request.IpAddress))
        {
            query = query.Where(l => l.IpAddress.Contains(request.IpAddress));
        }

        if (!string.IsNullOrEmpty(request.Location))
        {
            query = query.Where(l => l.Location != null && l.Location.Contains(request.Location));
        }

        if (request.StartTime.HasValue)
        {
            var startInstant = NodaTime.Instant.FromDateTimeUtc(request.StartTime.Value.ToUniversalTime());
            query = query.Where(l => l.CreatedAt >= startInstant);
        }

        if (request.EndTime.HasValue)
        {
            var endInstant = NodaTime.Instant.FromDateTimeUtc(request.EndTime.Value.ToUniversalTime());
            query = query.Where(l => l.CreatedAt <= endInstant);
        }

        // 排序
        query = ApplyOrdering(query, request.OrderBy, request.IsDescending);

        // 分页
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LoginLogDto
            {
                Id = l.Id,
                UserName = l.UserName,
                LoginType = l.LoginType,
                IsSuccess = l.IsSuccess,
                FailureReason = l.FailureReason,
                IpAddress = l.IpAddress,
                UserAgent = l.UserAgent,
                Browser = l.Browser,
                OperatingSystem = l.OperatingSystem,
                DeviceType = l.DeviceType,
                Location = l.Location,
                CreatedAt = l.CreatedAt.ToDateTimeUtc()
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<LoginLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result.Ok(result);
    }

    private static IQueryable<Domain.Entities.LoginLog> ApplyOrdering(
        IQueryable<Domain.Entities.LoginLog> query,
        string? orderBy,
        bool isDescending)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            return isDescending
                ? query.OrderByDescending(l => l.CreatedAt)
                : query.OrderBy(l => l.CreatedAt);
        }

        return orderBy.ToLower() switch
        {
            "username" => isDescending
                ? query.OrderByDescending(l => l.UserName)
                : query.OrderBy(l => l.UserName),
            "logintype" => isDescending
                ? query.OrderByDescending(l => l.LoginType)
                : query.OrderBy(l => l.LoginType),
            "issuccess" => isDescending
                ? query.OrderByDescending(l => l.IsSuccess)
                : query.OrderBy(l => l.IsSuccess),
            "createdat" => isDescending
                ? query.OrderByDescending(l => l.CreatedAt)
                : query.OrderBy(l => l.CreatedAt),
            _ => isDescending
                ? query.OrderByDescending(l => l.CreatedAt)
                : query.OrderBy(l => l.CreatedAt)
        };
    }
}
