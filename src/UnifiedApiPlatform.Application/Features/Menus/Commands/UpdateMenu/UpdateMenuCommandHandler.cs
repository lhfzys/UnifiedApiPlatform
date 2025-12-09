using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.UpdateMenu;

public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, Result<UpdateMenuResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<UpdateMenuCommandHandler> _logger;

    public UpdateMenuCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<UpdateMenuCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<UpdateMenuResponse>> Handle(
        UpdateMenuCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId;

            // 查询菜单
            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == request.MenuId &&
                    (m.TenantId == tenantId || m.TenantId == null),
                    cancellationToken);

            if (menu == null)
            {
                _logger.LogWarning("菜单不存在: {MenuId}", request.MenuId);
                return Result.Fail<UpdateMenuResponse>(ErrorCodes.MenuNotFound);
            }

            // 检查是否系统菜单
            if (menu.IsSystemMenu)
            {
                _logger.LogWarning("尝试修改系统菜单: {MenuId}", request.MenuId);
                return Result.Fail<UpdateMenuResponse>("不能修改系统菜单");
            }

            // 乐观并发控制
            if (request.RowVersion != null && !menu.RowVersion!.SequenceEqual(request.RowVersion))
            {
                _logger.LogWarning("菜单数据已被修改: {MenuId}", request.MenuId);
                return Result.Fail<UpdateMenuResponse>("菜单数据已被其他人修改，请刷新后重试");
            }

            // 更新字段（只更新提供的字段）
            if (request.ParentId.HasValue)
            {
                menu.ParentId = request.ParentId.Value == Guid.Empty ? null : request.ParentId;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                menu.Name = request.Name;
            }

            if (request.Type.HasValue)
            {
                menu.Type = request.Type.Value;
            }

            if (request.PermissionCode != null)
            {
                menu.PermissionCode = string.IsNullOrEmpty(request.PermissionCode) ? null : request.PermissionCode;
            }

            if (request.Icon != null)
            {
                menu.Icon = string.IsNullOrEmpty(request.Icon) ? null : request.Icon;
            }

            if (request.Path != null)
            {
                menu.Path = string.IsNullOrEmpty(request.Path) ? null : request.Path;
            }

            if (request.Component != null)
            {
                menu.Component = string.IsNullOrEmpty(request.Component) ? null : request.Component;
            }

            if (request.SortOrder.HasValue)
            {
                menu.SortOrder = request.SortOrder.Value;
            }

            if (request.IsVisible.HasValue)
            {
                menu.IsVisible = request.IsVisible.Value;
            }

            // 更新审计字段
            menu.UpdatedAt = _clock.GetCurrentInstant();
            menu.UpdatedBy = _currentUser.UserId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "菜单更新成功: {MenuId}, 名称: {Name}, 操作人: {OperatorId}, IP: {IpAddress}",
                menu.Id,
                menu.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok(new UpdateMenuResponse
            {
                Id = menu.Id,
                Code = menu.Code,
                Name = menu.Name
            });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "菜单更新冲突: {MenuId}", request.MenuId);
            return Result.Fail<UpdateMenuResponse>("菜单数据已被其他人修改，请刷新后重试");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "菜单更新失败: {MenuId}", request.MenuId);
            return Result.Fail<UpdateMenuResponse>("更新菜单失败");
        }
    }
}
