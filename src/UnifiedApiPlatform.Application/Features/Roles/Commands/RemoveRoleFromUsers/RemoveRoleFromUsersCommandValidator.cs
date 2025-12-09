using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.RemoveRoleFromUsers;

public class RemoveRoleFromUsersCommandValidator : AbstractValidator<RemoveRoleFromUsersCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveRoleFromUsersCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色 ID 不能为空")
            .MustAsync(RoleExists).WithMessage("角色不存在");

        RuleFor(x => x.UserIds)
            .NotNull().WithMessage("用户列表不能为空")
            .Must(x => x.Count > 0).WithMessage("至少选择一个用户");
    }

    private async Task<bool> RoleExists(Guid roleId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Roles.AnyAsync(r => r.Id == roleId && r.TenantId == tenantId, cancellationToken);
    }
}
