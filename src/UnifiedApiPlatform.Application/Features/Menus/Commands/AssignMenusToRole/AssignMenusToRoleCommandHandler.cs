using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.AssignMenusToRole;

public class AssignMenusToRoleCommandHandler : IRequestHandler<AssignMenusToRoleCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AssignMenusToRoleCommandHandler> _logger;

    public AssignMenusToRoleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<AssignMenusToRoleCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result> Handle(AssignMenusToRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 查询角色
            var role = await _context.Roles
                .Include(r => r.RoleMenus)
                .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.TenantId == tenantId,
                    cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail(ErrorCodes.RoleNotFound);
            }

            // 移除所有现有菜单
            var existingRoleMenus = role.RoleMenus.ToList();
            foreach (var roleMenu in existingRoleMenus)
            {
                _context.RoleMenus.Remove(roleMenu);
            }

            // 添加新菜单
            foreach (var menuId in request.MenuIds)
            {
                var roleMenu = new RoleMenu
                {
                    RoleId = role.Id,
                    MenuId = menuId
                };
                _context.RoleMenus.Add(roleMenu);
            }

            // 保存更改
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "角色菜单分配成功: {RoleId}, 菜单数量: {MenuCount}, 操作人: {OperatorId}, IP: {IpAddress}",
                role.Id,
                request.MenuIds.Count,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分配菜单失败: {RoleId}", request.RoleId);
            return Result.Fail("分配菜单失败");
        }
    }
}
