using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizations;

public class GetOrganizationsQueryHandler: IRequestHandler<GetOrganizationsQuery, Result<List<OrganizationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetOrganizationsQueryHandler> _logger;

    public GetOrganizationsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetOrganizationsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<OrganizationDto>>> Handle(
        GetOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Organizations
                .AsNoTracking()
                .Where(o => o.TenantId == request.CurrentTenantId);

            // 搜索
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                query = query.Where(o =>
                    o.Name.ToLower().Contains(keyword) ||
                    o.Code.ToLower().Contains(keyword));
            }

            // 父组织筛选
            if (request.ParentId.HasValue)
            {
                query = query.Where(o => o.ParentId == request.ParentId.Value);
            }

            // 激活状态筛选
            if (request.IsActive.HasValue)
            {
                query = query.Where(o => o.IsDeleted == request.IsActive.Value);
            }
// 查询组织
            var organizations = await query
                .OrderBy(o => o.SortOrder)
                .ThenBy(o => o.Name)
                .Select(o => new OrganizationDto
                {
                    Id = o.Id,
                    ParentId = o.ParentId,
                    Code = o.Code,
                    Name = o.Name,
                    FullName = o.FullName,
                    Description = o.Description,
                    SortOrder = o.SortOrder,
                    IsActive = o.IsActive,
                    UserCount = o.Users.Count,
                    ChildCount = o.Children.Count,
                    CreatedAt = o.CreatedAt.ToDateTimeUtc()
                })
                .ToListAsync(cancellationToken);

            return Result.Ok(organizations);

        }catch (Exception ex)
        {
            _logger.LogError(ex, "获取组织列表失败");
            return Result.Fail<List<OrganizationDto>>("获取组织列表失败");
        }
    }
}
