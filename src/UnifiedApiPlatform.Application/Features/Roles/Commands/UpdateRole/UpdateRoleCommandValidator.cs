using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateRoleCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色 ID 不能为空")
            .MustAsync(RoleExists).WithMessage("角色不存在")
            .MustAsync(NotSystemRole).WithMessage("不能修改系统角色");

        RuleFor(x => x.DisplayName)
            .Length(2, 100).WithMessage("显示名称长度必须在 2-100 个字符之间")
            .When(x => !string.IsNullOrEmpty(x.DisplayName));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));
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
}
