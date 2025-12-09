using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

public class GetMenusQueryHandler : IRequestHandler<GetMenusQuery, Result<List<MenuDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMenusQueryHandler> _logger;

    public GetMenusQueryHandler(
        IApplicationDbContext context,
        ILogger<GetMenusQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<MenuDto>>> Handle(
        GetMenusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Menus
                .AsNoTracking()
                .Where(m => m.TenantId == request.CurrentTenantId || m.TenantId == null); // 系统菜单或租户菜单

            // 搜索
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                query = query.Where(m =>
                    m.Name.ToLower().Contains(keyword) ||
                    m.Code.ToLower().Contains(keyword));
            }

            // 父菜单筛选
            if (request.ParentId.HasValue)
            {
                query = query.Where(m => m.ParentId == request.ParentId.Value);
            }

            // 类型筛选
            if (request.Type.HasValue)
            {
                query = query.Where(m => m.Type == request.Type);
            }

            // 可见性筛选
            if (request.IsVisible.HasValue)
            {
                query = query.Where(m => m.IsVisible == request.IsVisible.Value);
            }

            // 激活状态筛选
            if (request.IsActive.HasValue)
            {
                query = query.Where(m => m.IsActive == request.IsActive.Value);
            }

            // 查询菜单
            var menus = await query
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.Name)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    Code = m.Code,
                    Name = m.Name,
                    Type = m.Type,
                    PermissionCode = m.PermissionCode,
                    Icon = m.Icon,
                    Path = m.Path,
                    Component = m.Component,
                    SortOrder = m.SortOrder,
                    IsVisible = m.IsVisible,
                    IsActive = m.IsActive,
                    IsSystemMenu = m.IsSystemMenu,
                    ChildCount = m.Children.Count,
                    CreatedAt = m.CreatedAt.ToDateTimeUtc()
                })
                .ToListAsync(cancellationToken);

            return Result.Ok(menus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单列表失败");
            return Result.Fail<List<MenuDto>>("获取菜单列表失败");
        }
    }
}
