using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateRoleCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空")
            .Length(2, 50).WithMessage("角色名称长度必须在 2-50 个字符之间")
            .Matches("^[a-zA-Z][a-zA-Z0-9_]*$").WithMessage("角色名称必须以字母开头，只能包含字母、数字和下划线")
            .MustAsync(BeUniqueName).WithMessage("角色名称已存在");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("显示名称不能为空")
            .Length(2, 100).WithMessage("显示名称长度必须在 2-100 个字符之间");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.PermissionCodes)
            .MustAsync(AllPermissionsExist!).WithMessage("部分权限不存在")
            .When(x => x.PermissionCodes != null && x.PermissionCodes.Count > 0);
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";

        return !await _context.Roles
            .AnyAsync(r => r.Name == name && r.TenantId == tenantId, cancellationToken);
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
