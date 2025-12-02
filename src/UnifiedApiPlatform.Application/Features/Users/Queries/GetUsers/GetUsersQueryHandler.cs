using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUsersQueryHandler> _logger;
    private readonly IClock _clock;

    public GetUsersQueryHandler(
        IApplicationDbContext context,
        ILogger<GetUsersQueryHandler> logger, IClock clock)
    {
        _context = context;
        _logger = logger;
        _clock = clock;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 基础查询（自动应用租户过滤和软删除过滤）
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.TenantId == request.CurrentTenantId);

            // 搜索关键字
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                query = query.Where(u =>
                            u.UserName.ToLower().Contains(keyword) ||
                            u.Email.ToLower().Contains(keyword) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(keyword))
                );
            }

            // 状态筛选
            if (request.Status.HasValue)
            {
                query = query.Where(u => u.Status == request.Status.Value);
            }

            // 激活状态筛选
            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            // 组织筛选
            if (request.OrganizationId.HasValue)
            {
                query = query.Where(u => u.OrganizationId == request.OrganizationId.Value);
            }

            // 角色筛选
            if (request.RoleId.HasValue)
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value));
            }

            // 排序
            query = ApplySorting(query, request.SortBy, request.SortDescending);

            // 获取总数
            var totalCount = await query.CountAsync(cancellationToken);

            // 分页
            var users = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(u => u.Organization)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync(cancellationToken);

            // 映射到 DTO
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                Status = u.Status,
                IsActive = u.IsActive,
                IsLocked = u.LockedUntil.HasValue && u.LockedUntil.Value > _clock.GetCurrentInstant(),
                LastLoginAt = u.LastLoginAt,
                LastLoginIp = u.LastLoginIp,
                CreatedAt = u.CreatedAt,
                OrganizationId = u.OrganizationId,
                OrganizationName = u.Organization?.Name,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();

            // 创建分页结果
            var result = PagedResult<UserDto>.Create(
                userDtos,
                totalCount,
                request.PageIndex,
                request.PageSize
            );

            _logger.LogInformation(
                "查询用户列表成功，返回 {Count} 条记录，共 {TotalCount} 条",
                userDtos.Count,
                totalCount
            );

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询用户列表失败");
            return Result.Fail("查询用户列表失败");
        }
    }

    private static IQueryable<Domain.Entities.User> ApplySorting(
        IQueryable<Domain.Entities.User> query,
        string? sortBy,
        bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            // 默认按创建时间倒序
            return query.OrderByDescending(u => u.CreatedAt);
        }

        // 根据排序字段排序
        query = sortBy.ToLower() switch
        {
            "username" => sortDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),
            "email" => sortDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            "createdat" => sortDescending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt),
            "lastloginat" => sortDescending
                ? query.OrderByDescending(u => u.LastLoginAt)
                : query.OrderBy(u => u.LastLoginAt),
            _ => query.OrderByDescending(u => u.CreatedAt)
        };

        return query;
    }
}
