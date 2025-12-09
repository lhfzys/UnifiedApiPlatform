using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.DeleteMenu;

public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<DeleteMenuCommandHandler> _logger;

    public DeleteMenuCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<DeleteMenuCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId;

            // 查询菜单（包含角色菜单关联）
            var menu = await _context.Menus
                .Include(m => m.RoleMenus)
                .FirstOrDefaultAsync(m => m.Id == request.MenuId &&
                    (m.TenantId == tenantId || m.TenantId == null),
                    cancellationToken);

            if (menu == null)
            {
                _logger.LogWarning("菜单不存在: {MenuId}", request.MenuId);
                return Result.Fail(ErrorCodes.MenuNotFound);
            }

            // 软删除菜单
            menu.IsDeleted = true;
            menu.DeletedAt = _clock.GetCurrentInstant();
            menu.DeletedBy = _currentUser.UserId;

            // 删除角色菜单关联
            foreach (var roleMenu in menu.RoleMenus.ToList())
            {
                _context.RoleMenus.Remove(roleMenu);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "菜单删除成功: {MenuId}, 名称: {Name}, 操作人: {OperatorId}, IP: {IpAddress}",
                menu.Id,
                menu.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除菜单失败: {MenuId}", request.MenuId);
            return Result.Fail("删除菜单失败");
        }
    }
}
