using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.UpdateMenu;

public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateMenuCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单 ID 不能为空")
            .MustAsync(MenuExists).WithMessage("菜单不存在")
            .MustAsync(NotSystemMenu).WithMessage("不能修改系统菜单");

        RuleFor(x => x.Name)
            .Length(2, 100).WithMessage("菜单名称长度必须在 2-100 个字符之间")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("无效的菜单类型")
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("图标长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.Icon));

        RuleFor(x => x.Path)
            .MaximumLength(200).WithMessage("路由路径长度不能超过 200 个字符")
            .When(x => !string.IsNullOrEmpty(x.Path));

        RuleFor(x => x.Component)
            .MaximumLength(200).WithMessage("组件路径长度不能超过 200 个字符")
            .When(x => !string.IsNullOrEmpty(x.Component));

        RuleFor(x => x.PermissionCode)
            .MaximumLength(100).WithMessage("权限代码长度不能超过 100 个字符")
            .MustAsync(async (permissionCode, cancellationToken) =>
                await PermissionExists(permissionCode, cancellationToken))
            .WithMessage("权限不存在")
            .When(x => !string.IsNullOrEmpty(x.PermissionCode));

        RuleFor(x => x)
            .MustAsync(ParentIsValid).WithMessage("不能将自己或子菜单设为父菜单")
            .When(x => x.ParentId.HasValue);
    }

    private async Task<bool> MenuExists(Guid menuId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;

        return await _context.Menus
            .AnyAsync(m => m.Id == menuId && (m.TenantId == tenantId || m.TenantId == null),
                cancellationToken);
    }

    private async Task<bool> NotSystemMenu(Guid menuId, CancellationToken cancellationToken)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(m => m.Id == menuId, cancellationToken);

        return menu != null && !menu.IsSystemMenu;
    }

    private async Task<bool> PermissionExists(string? permissionCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(permissionCode)) return true;

        var tenantId = _currentUser.TenantId ?? "default";

        return await _context.Permissions
            .AnyAsync(p => p.Code == permissionCode && p.TenantId == tenantId,
                cancellationToken);
    }

    private async Task<bool> ParentIsValid(UpdateMenuCommand command, CancellationToken cancellationToken)
    {
        if (!command.ParentId.HasValue) return true;

        var tenantId = _currentUser.TenantId;

        // 不能将自己设为父菜单
        if (command.ParentId.Value == command.MenuId)
        {
            return false;
        }

        // 检查父菜单是否存在
        var parent = await _context.Menus
            .FirstOrDefaultAsync(m => m.Id == command.ParentId.Value &&
                (m.TenantId == tenantId || m.TenantId == null), cancellationToken);

        if (parent == null) return false;

        // 检查是否会形成循环（简单检查：检查父菜单的父菜单链）
        var currentParentId = parent.ParentId;
        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == command.MenuId)
            {
                return false; // 会形成循环
            }

            var nextParent = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == currentParentId.Value, cancellationToken);

            currentParentId = nextParent?.ParentId;
        }

        return true;
    }
}
