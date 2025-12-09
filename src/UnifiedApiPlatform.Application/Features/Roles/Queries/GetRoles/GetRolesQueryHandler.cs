using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<PagedResult<RoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRolesQueryHandler> _logger;

    public GetRolesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetRolesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PagedResult<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Roles
                .AsNoTracking()
                .Where(r => r.TenantId == request.CurrentTenantId);

            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                query = query.Where(r =>
                    r.Name.ToLower().Contains(keyword) ||
                    r.DisplayName.ToLower().Contains(keyword));
            }

            if (request.IsSystemRole.HasValue)
            {
                query = query.Where(r => r.IsSystemRole == request.IsSystemRole.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, request.SortBy, request.SortDescending);
            // 分页
            var roles = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    DisplayName = r.DisplayName,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    UserCount = r.UserRoles.Count,
                    PermissionCount = r.RolePermissions.Count,
                    CreatedAt = r.CreatedAt.ToDateTimeUtc()
                })
                .ToListAsync(cancellationToken);

            var result = new PagedResult<RoleDto>
            {
                Items = roles, TotalCount = totalCount, PageIndex = request.PageIndex, PageSize = request.PageSize
            };

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色列表失败");
            return Result.Fail<PagedResult<RoleDto>>("获取角色列表失败");
        }
    }

    private static IQueryable<Domain.Entities.Role> ApplySorting(
        IQueryable<Domain.Entities.Role> query,
        string? sortBy,
        bool isDescending)
    {
        var sortByLower = sortBy?.ToLower();

        query = sortByLower switch
        {
            "name" => isDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
            "displayname" => isDescending
                ? query.OrderByDescending(r => r.DisplayName)
                : query.OrderBy(r => r.DisplayName),
            "createdat" => isDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            _ => query.OrderBy(r => r.Name)
        };

        return query;
    }
}
