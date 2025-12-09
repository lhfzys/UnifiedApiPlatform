using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.AssignMenusToRole;

public class AssignMenusToRoleCommandValidator : AbstractValidator<AssignMenusToRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AssignMenusToRoleCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色 ID 不能为空")
            .MustAsync(RoleExists).WithMessage("角色不存在")
            .MustAsync(NotSystemRole).WithMessage("不能修改系统角色的菜单");

        RuleFor(x => x.MenuIds)
            .NotNull().WithMessage("菜单列表不能为空")
            .MustAsync(AllMenusExist).WithMessage("部分菜单不存在");
    }

    private async Task<bool> RoleExists(Guid roleId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Roles.AnyAsync(r => r.Id == roleId && r.TenantId == tenantId,
            cancellationToken);
    }

    private async Task<bool> NotSystemRole(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        return role != null && !role.IsSystemRole;
    }

    private async Task<bool> AllMenusExist(List<Guid> menuIds, CancellationToken cancellationToken)
    {
        if (menuIds.Count == 0) return true;

        var tenantId = _currentUser.TenantId;

        var existingMenuCount = await _context.Menus
            .Where(m => menuIds.Contains(m.Id) && (m.TenantId == tenantId || m.TenantId == null))
            .CountAsync(cancellationToken);

        return existingMenuCount == menuIds.Count;
    }
}
