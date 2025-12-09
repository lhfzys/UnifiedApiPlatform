using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetCurrentUserMenus;

public class GetCurrentUserMenusQueryHandler : IRequestHandler<GetCurrentUserMenusQuery, Result<List<MenuTreeNodeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetCurrentUserMenusQueryHandler> _logger;

    public GetCurrentUserMenusQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<GetCurrentUserMenusQueryHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<List<MenuTreeNodeDto>>> Handle(
        GetCurrentUserMenusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            // 查询用户的角色
            var userRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            if (userRoleIds.Count == 0)
            {
                return Result.Ok(new List<MenuTreeNodeDto>());
            }

            // 查询角色的菜单 ID
            var menuIds = await _context.RoleMenus
                .Where(rm => userRoleIds.Contains(rm.RoleId))
                .Select(rm => rm.MenuId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (menuIds.Count == 0)
            {
                return Result.Ok(new List<MenuTreeNodeDto>());
            }

            // 查询菜单详情
            var menus = await _context.Menus
                .AsNoTracking()
                .Where(m => menuIds.Contains(m.Id))
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
            _logger.LogError(ex, "获取当前用户菜单失败");
            return Result.Fail<List<MenuTreeNodeDto>>("获取当前用户菜单失败");
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
