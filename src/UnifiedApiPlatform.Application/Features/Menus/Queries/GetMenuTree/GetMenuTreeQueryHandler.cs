using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenuTree;

public class GetMenuTreeQueryHandler : IRequestHandler<GetMenuTreeQuery, Result<List<MenuTreeNodeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMenuTreeQueryHandler> _logger;

    public GetMenuTreeQueryHandler(
        IApplicationDbContext context,
        ILogger<GetMenuTreeQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<MenuTreeNodeDto>>> Handle(
        GetMenuTreeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Menus
                .AsNoTracking()
                .Where(m => m.TenantId == request.CurrentTenantId || m.TenantId == null);

            // 是否包含未激活的
            if (!request.IncludeInactive)
            {
                query = query.Where(m => m.IsActive);
            }

            // 是否包含不可见的
            if (!request.IncludeHidden)
            {
                query = query.Where(m => m.IsVisible);
            }

            // 查询所有菜单
            var allMenus = await query
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.Name)
                .Select(m => new MenuTreeNodeDto
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
                    Children = new List<MenuTreeNodeDto>()
                })
                .ToListAsync(cancellationToken);

            // 构建树形结构
            var tree = BuildTree(allMenus, null);

            return Result.Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单树失败");
            return Result.Fail<List<MenuTreeNodeDto>>("获取菜单树失败");
        }
    }

    /// <summary>
    /// 构建树形结构（递归）
    /// </summary>
    private static List<MenuTreeNodeDto> BuildTree(
        List<MenuTreeNodeDto> allNodes,
        Guid? parentId)
    {
        return allNodes
            .Where(n => n.ParentId == parentId)
            .Select(n =>
            {
                n.Children = BuildTree(allNodes, n.Id);
                return n;
            })
            .OrderBy(n => n.SortOrder)
            .ThenBy(n => n.Name)
            .ToList();
    }
}
