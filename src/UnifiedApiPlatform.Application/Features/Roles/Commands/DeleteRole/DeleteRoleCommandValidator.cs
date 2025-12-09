using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteRoleCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色 ID 不能为空")
            .MustAsync(RoleExists).WithMessage("角色不存在")
            .MustAsync(NotSystemRole).WithMessage("不能删除系统角色")
            .MustAsync(NotInUse).WithMessage("角色正在使用中，不能删除");
    }

    private async Task<bool> RoleExists(Guid roleId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Roles.AnyAsync(r => r.Id == roleId && r.TenantId == tenantId, cancellationToken);
    }

    private async Task<bool> NotSystemRole(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        return role != null && !role.IsSystemRole;
    }

    private async Task<bool> NotInUse(Guid roleId, CancellationToken cancellationToken)
    {
        // 检查是否有用户使用该角色
        var hasUsers = await _context.UserRoles
            .AnyAsync(ur => ur.RoleId == roleId, cancellationToken);

        return !hasUsers;
    }
}
