using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationUsers;

public class
    GetOrganizationUsersQueryHandler : IRequestHandler<GetOrganizationUsersQuery,
    Result<PagedResult<OrganizationUserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetOrganizationUsersQueryHandler> _logger;

    public GetOrganizationUsersQueryHandler(
        IApplicationDbContext context,
        ILogger<GetOrganizationUsersQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PagedResult<OrganizationUserDto>>> Handle(
        GetOrganizationUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 验证组织存在
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == request.OrganizationId && o.TenantId == request.CurrentTenantId,
                    cancellationToken);

            if (organization == null)
            {
                _logger.LogWarning("组织不存在: {OrganizationId}", request.OrganizationId);
                return Result.Fail<PagedResult<OrganizationUserDto>>(ErrorCodes.OrganizationNotFound);
            }

            // 构建查询
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.TenantId == request.CurrentTenantId);

            // 是否包含子组织
            if (request.IncludeChildren)
            {
                // 查询当前组织及其所有子组织的用户
                var childOrganizationIds = await _context.Organizations
                    .Where(o => o.Path.StartsWith(organization.Path))
                    .Select(o => o.Id)
                    .ToListAsync(cancellationToken);

                query = query.Where(u =>
                    u.OrganizationId.HasValue && childOrganizationIds.Contains(u.OrganizationId.Value));
            }
            else
            {
                // 只查询当前组织的用户
                query = query.Where(u => u.OrganizationId == request.OrganizationId);
            }

            // 搜索
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword));
            }

            // 筛选激活状态
            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            // 总数
            var totalCount = await query.CountAsync(cancellationToken);

            // 排序
            query = ApplySorting(query, request.SortBy, request.SortDescending);

            // 分页并投影
            var users = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new OrganizationUserDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    IsActive = u.IsActive,
                    ManagerName = u.Manager != null ? u.Manager.UserName : null,
                    Roles = u.UserRoles.Select(ur => ur.Role.DisplayName).ToList(),
                    CreatedAt = u.CreatedAt.ToDateTimeUtc()
                })
                .ToListAsync(cancellationToken);

            var result = new PagedResult<OrganizationUserDto>
            {
                Items = users, TotalCount = totalCount, PageIndex = request.PageIndex, PageSize = request.PageSize
            };

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组织用户列表失败: {OrganizationId}", request.OrganizationId);
            return Result.Fail<PagedResult<OrganizationUserDto>>("获取组织用户列表失败");
        }
    }

    private static IQueryable<Domain.Entities.User> ApplySorting(
        IQueryable<Domain.Entities.User> query,
        string? sortBy,
        bool isDescending)
    {
        var sortByLower = sortBy?.ToLower();

        return sortByLower switch
        {
            "username" => isDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),

            "email" => isDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),

            "createdat" => isDescending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt),

            _ => query.OrderBy(u => u.UserName)
        };
    }
}
