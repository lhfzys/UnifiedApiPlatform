using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetRoleMenus;

public class GetRoleMenusQueryHandler : IRequestHandler<GetRoleMenusQuery, Result<List<MenuTreeNodeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRoleMenusQueryHandler> _logger;

    public GetRoleMenusQueryHandler(
        IApplicationDbContext context,
        ILogger<GetRoleMenusQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<MenuTreeNodeDto>>> Handle(
        GetRoleMenusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 验证角色存在
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Id == request.RoleId && r.TenantId == request.CurrentTenantId,
                    cancellationToken);

            if (!roleExists)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail<List<MenuTreeNodeDto>>(ErrorCodes.RoleNotFound);
            }

            // 查询角色的菜单
            var roleMenuIds = await _context.RoleMenus
                .Where(rm => rm.RoleId == request.RoleId)
                .Select(rm => rm.MenuId)
                .ToListAsync(cancellationToken);

            if (roleMenuIds.Count == 0)
            {
                return Result.Ok(new List<MenuTreeNodeDto>());
            }

            // 查询菜单详情
            var menus = await _context.Menus
                .AsNoTracking()
                .Where(m => roleMenuIds.Contains(m.Id))
                .Where(m => m.IsActive && m.IsVisible)
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
            var tree = BuildTree(menus, null);

            return Result.Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色菜单失败: {RoleId}", request.RoleId);
            return Result.Fail<List<MenuTreeNodeDto>>("获取角色菜单失败");
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
