using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.AssignPermissions;

public class AssignPermissionsCommandValidator : AbstractValidator<AssignPermissionsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AssignPermissionsCommandValidator(IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色 ID 不能为空")
            .MustAsync(RoleExists).WithMessage("角色不存在")
            .MustAsync(NotSystemRole).WithMessage("不能修改系统角色的权限");

        RuleFor(x => x.PermissionCodes)
            .NotNull().WithMessage("权限列表不能为空")
            .MustAsync(AllPermissionsExist).WithMessage("部分权限不存在");
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

    private async Task<bool> AllPermissionsExist(List<string> permissionCodes, CancellationToken cancellationToken)
    {
        if (permissionCodes.Count == 0) return true;

        var tenantId = _currentUser.TenantId ?? "default";

        var existingPermissionCount = await _context.Permissions
            .Where(p => permissionCodes.Contains(p.Code) && p.TenantId == tenantId)
            .CountAsync(cancellationToken);

        return existingPermissionCount == permissionCodes.Count;
    }
}
