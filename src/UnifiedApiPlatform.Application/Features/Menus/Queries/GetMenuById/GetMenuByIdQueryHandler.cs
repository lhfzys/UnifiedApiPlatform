using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenuById;

public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, Result<MenuDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMenuByIdQueryHandler> _logger;

    public GetMenuByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetMenuByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<MenuDetailDto>> Handle(
        GetMenuByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var menu = await _context.Menus
                .AsNoTracking()
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == request.MenuId &&
                    (m.TenantId == request.CurrentTenantId || m.TenantId == null),
                    cancellationToken);

            if (menu == null)
            {
                _logger.LogWarning("菜单不存在: {MenuId}", request.MenuId);
                return Result.Fail<MenuDetailDto>(ErrorCodes.MenuNotFound);
            }

            var dto = new MenuDetailDto
            {
                Id = menu.Id,
                ParentId = menu.ParentId,
                ParentName = menu.Parent?.Name,
                Code = menu.Code,
                Name = menu.Name,
                Type = menu.Type,
                PermissionCode = menu.PermissionCode,
                Icon = menu.Icon,
                Path = menu.Path,
                Component = menu.Component,
                SortOrder = menu.SortOrder,
                IsVisible = menu.IsVisible,
                IsActive = menu.IsActive,
                IsSystemMenu = menu.IsSystemMenu,
                ChildCount = await _context.Menus.CountAsync(m => m.ParentId == menu.Id, cancellationToken),
                RoleCount = await _context.RoleMenus.CountAsync(rm => rm.MenuId == menu.Id, cancellationToken),
                CreatedAt = menu.CreatedAt.ToDateTimeUtc(),
                CreatedBy = menu.CreatedBy,
                UpdatedAt = menu.UpdatedAt?.ToDateTimeUtc(),
                UpdatedBy = menu.UpdatedBy,
                RowVersion = menu.RowVersion != null ? Convert.ToBase64String(menu.RowVersion) : null
            };

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单详情失败: {MenuId}", request.MenuId);
            return Result.Fail<MenuDetailDto>("获取菜单详情失败");
        }
    }
}
