using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Shared.Constants;
using NodaTime;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleUsers;

public class GetRoleUsersQueryHandler : IRequestHandler<GetRoleUsersQuery, Result<PagedResult<RoleUserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRoleUsersQueryHandler> _logger;

    public GetRoleUsersQueryHandler(
        IApplicationDbContext context,
        ILogger<GetRoleUsersQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PagedResult<RoleUserDto>>> Handle(
        GetRoleUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 验证角色存在
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Id == request.RoleId && r.TenantId == request.CurrentTenantId, cancellationToken);

            if (!roleExists)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail<PagedResult<RoleUserDto>>(ErrorCodes.RoleNotFound);
            }

            // 构建基础查询
            var baseQuery = _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.RoleId == request.RoleId)
                .Where(ur => ur.User.TenantId == request.CurrentTenantId);

            // 应用搜索和筛选
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                baseQuery = baseQuery.Where(ur =>
                    ur.User.UserName.ToLower().Contains(keyword) ||
                    ur.User.Email.ToLower().Contains(keyword));
            }

            if (request.IsActive.HasValue)
            {
                baseQuery = baseQuery.Where(ur => ur.User.IsActive == request.IsActive.Value);
            }

            // 总数
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var sortedQuery = ApplySorting(baseQuery, request.SortBy, request.SortDescending);

            // 分页并投影到 DTO
            var users = await sortedQuery
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(ur => new RoleUserDto
                {
                    UserId = ur.User.Id,
                    UserName = ur.User.UserName,
                    Email = ur.User.Email,
                    Avatar = ur.User.Avatar,
                    IsActive = ur.User.IsActive,
                    OrganizationName = ur.User.Organization != null ? ur.User.Organization.Name : null,
                    AssignedAt = ur.CreatedAt.ToDateTimeUtc()
                })
                .ToListAsync(cancellationToken);

            var result = new PagedResult<RoleUserDto>
            {
                Items = users, TotalCount = totalCount, PageIndex = request.PageIndex, PageSize = request.PageSize
            };

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色用户列表失败: {RoleId}", request.RoleId);
            return Result.Fail<PagedResult<RoleUserDto>>("获取角色用户列表失败");
        }
    }

    /// <summary>
    /// 应用排序
    /// </summary>
    private static IQueryable<UserRole> ApplySorting(
        IQueryable<UserRole> query,
        string? sortBy,
        bool isDescending)
    {
        var sortByLower = sortBy?.ToLower();

        return sortByLower switch
        {
            "username" => isDescending
                ? query.OrderByDescending(ur => ur.User.UserName)
                : query.OrderBy(ur => ur.User.UserName),

            "email" => isDescending
                ? query.OrderByDescending(ur => ur.User.Email)
                : query.OrderBy(ur => ur.User.Email),

            "assignedat" => isDescending
                ? query.OrderByDescending(ur => ur.CreatedAt)
                : query.OrderBy(ur => ur.CreatedAt),

            _ => query.OrderBy(ur => ur.User.UserName) // 默认按用户名排序
        };
    }
}
