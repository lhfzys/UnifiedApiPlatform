using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAuditLogsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        // 构建查询
        var query = _context.AuditLogs.AsQueryable();

        // 筛选条件
        if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(a => a.UserName != null && a.UserName.Contains(request.UserName));
        }

        if (!string.IsNullOrEmpty(request.Action))
        {
            query = query.Where(a => a.Action == request.Action);
        }

        if (!string.IsNullOrEmpty(request.EntityType))
        {
            query = query.Where(a => a.EntityType == request.EntityType);
        }

        if (!string.IsNullOrEmpty(request.HttpMethod))
        {
            query = query.Where(a => a.HttpMethod == request.HttpMethod);
        }

        if (!string.IsNullOrEmpty(request.RequestPath))
        {
            query = query.Where(a => a.RequestPath.Contains(request.RequestPath));
        }

        if (request.IsSuccess.HasValue)
        {
            query = query.Where(a => a.IsSuccess == request.IsSuccess.Value);
        }

        if (request.StatusCode.HasValue)
        {
            query = query.Where(a => a.StatusCode == request.StatusCode.Value);
        }

        if (!string.IsNullOrEmpty(request.IpAddress))
        {
            query = query.Where(a => a.IpAddress.Contains(request.IpAddress));
        }

        if (request.StartTime.HasValue)
        {
            var startInstant = NodaTime.Instant.FromDateTimeUtc(request.StartTime.Value.ToUniversalTime());
            query = query.Where(a => a.CreatedAt >= startInstant);
        }

        if (request.EndTime.HasValue)
        {
            var endInstant = NodaTime.Instant.FromDateTimeUtc(request.EndTime.Value.ToUniversalTime());
            query = query.Where(a => a.CreatedAt <= endInstant);
        }

        // 排序
        query = ApplyOrdering(query, request.OrderBy, request.IsDescending);

        // 分页
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                UserName = a.UserName,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                HttpMethod = a.HttpMethod,
                RequestPath = a.RequestPath,
                RequestBody = a.RequestBody,
                StatusCode = a.StatusCode,
                ResponseBody = a.ResponseBody,
                Duration = a.Duration,
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                Exception = a.Exception,
                IsSuccess = a.IsSuccess,
                CreatedAt = a.CreatedAt.ToDateTimeUtc()
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result.Ok(result);
    }

    private static IQueryable<Domain.Entities.AuditLog> ApplyOrdering(
        IQueryable<Domain.Entities.AuditLog> query,
        string? orderBy,
        bool isDescending)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            return isDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt);
        }

        return orderBy.ToLower() switch
        {
            "username" => isDescending
                ? query.OrderByDescending(a => a.UserName)
                : query.OrderBy(a => a.UserName),
            "action" => isDescending
                ? query.OrderByDescending(a => a.Action)
                : query.OrderBy(a => a.Action),
            "statuscode" => isDescending
                ? query.OrderByDescending(a => a.StatusCode)
                : query.OrderBy(a => a.StatusCode),
            "duration" => isDescending
                ? query.OrderByDescending(a => a.Duration)
                : query.OrderBy(a => a.Duration),
            "createdat" => isDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt),
            _ => isDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt)
        };
    }
}
