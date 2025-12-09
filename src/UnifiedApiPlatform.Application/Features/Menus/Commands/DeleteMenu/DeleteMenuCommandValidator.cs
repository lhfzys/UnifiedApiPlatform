using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.DeleteMenu;

public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteMenuCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单 ID 不能为空")
            .MustAsync(MenuExists).WithMessage("菜单不存在")
            .MustAsync(NotSystemMenu).WithMessage("不能删除系统菜单")
            .MustAsync(HasNoChildren).WithMessage("菜单存在子菜单，不能删除");
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

    private async Task<bool> HasNoChildren(Guid menuId, CancellationToken cancellationToken)
    {
        var hasChildren = await _context.Menus
            .AnyAsync(m => m.ParentId == menuId, cancellationToken);

        return !hasChildren;
    }
}
