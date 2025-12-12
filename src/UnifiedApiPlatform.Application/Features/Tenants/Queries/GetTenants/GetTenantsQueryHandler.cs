using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Tenants.Queries.GetTenants;

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, Result<PagedResult<TenantDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTenantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<TenantDto>>> Handle(
        GetTenantsQuery request,
        CancellationToken cancellationToken)
    {
        // 构建查询
        var query = _context.Tenants
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        // 搜索关键字
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            query = query.Where(t =>
                t.Name.Contains(request.SearchKeyword) ||
                t.Identifier.Contains(request.SearchKeyword));
        }

        // 按编码筛选
        if (!string.IsNullOrEmpty(request.Identifier))
        {
            query = query.Where(t => t.Identifier == request.Identifier);
        }

        // 按状态筛选
        if (request.IsActive.HasValue)
        {
            query = query.Where(t => t.IsActive == request.IsActive.Value);
        }

        // 排序
        query = ApplyOrdering(query, request.OrderBy, request.IsDescending);

        // 分页
        var totalCount = await query.CountAsync(cancellationToken);

        var tenants = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new
            {
                Tenant = t,
                UserCount = _context.Users.Count(u => u.TenantId == t.Id.ToString() && !u.IsDeleted),
                RoleCount = _context.Roles.Count(r => r.TenantId == t.Id.ToString() && !r.IsDeleted),
                OrganizationCount = _context.Organizations.Count(o => o.TenantId == t.Id.ToString() && !o.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        var items = tenants.Select(x => new TenantDto
        {
            Id = x.Tenant.Id.ToString(),
            Identifier = x.Tenant.Identifier,
            Name = x.Tenant.Name,
            Description = x.Tenant.Description,
            ContactEmail = x.Tenant.ContactEmail,
            ContactPhone = x.Tenant.ContactPhone,
            IsActive = x.Tenant.IsActive,
            CreatedAt = x.Tenant.CreatedAt.ToDateTimeUtc(),
            UpdatedAt = x.Tenant.UpdatedAt?.ToDateTimeUtc(),
            Statistics = new TenantStatistics
            {
                UserCount = x.UserCount,
                RoleCount = x.RoleCount,
                OrganizationCount = x.OrganizationCount
            }
        }).ToList();

        var result = new PagedResult<TenantDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result.Ok(result);
    }

    private static IQueryable<Domain.Entities.Tenant> ApplyOrdering(
        IQueryable<Domain.Entities.Tenant> query,
        string? orderBy,
        bool isDescending)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            return isDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt);
        }

        return orderBy.ToLower() switch
        {
            "Identifier" => isDescending
                ? query.OrderByDescending(t => t.Identifier)
                : query.OrderBy(t => t.Identifier),
            "name" => isDescending
                ? query.OrderByDescending(t => t.Name)
                : query.OrderBy(t => t.Name),
            "createdat" => isDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),
            _ => isDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };
    }
}
