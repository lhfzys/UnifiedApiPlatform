using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Permissions.Queries.GetPermissions;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<List<PermissionCategoryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    public GetPermissionsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetPermissionsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<PermissionCategoryDto>>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Permissions
                .AsNoTracking()
                .Where(p => p.TenantId == request.CurrentTenantId);

            // 搜索
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                query = query.Where(p =>
                    p.Code.ToLower().Contains(keyword) ||
                    p.Name.ToLower().Contains(keyword) ||
                    (p.Description != null && p.Description.ToLower().Contains(keyword)));
            }

            // 分类筛选
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                query = query.Where(p => p.Category == request.Category);
            }

            // 查询权限
            var permissions = await query
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Code)
                .ToListAsync(cancellationToken);

            // 按分类分组
            var grouped = permissions
                .GroupBy(p => p.Category)
                .Select(g => new PermissionCategoryDto
                {
                    Category = g.Key,
                    Permissions = g.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        Category = p.Category,
                        Description = p.Description,
                        IsSystemPermission = p.IsSystemPermission
                    }).ToList()
                })
                .ToList();

            _logger.LogInformation("成功获取 {Count} 个分类的权限", grouped.Count);

            return Result.Ok(grouped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取权限列表失败");
            return Result.Fail<List<PermissionCategoryDto>>("获取权限列表失败");
        }
    }
}
