using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.AssignRolesToUsers;

public class AssignRolesToUsersCommandValidator : AbstractValidator<AssignRolesToUsersCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AssignRolesToUsersCommandValidator(
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
            .Must(x => x.Count > 0).WithMessage("至少选择一个用户")
            .MustAsync(AllUsersExist).WithMessage("部分用户不存在");
    }

    private async Task<bool> RoleExists(Guid roleId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Roles.AnyAsync(r => r.Id == roleId && r.TenantId == tenantId, cancellationToken);
    }

    private async Task<bool> AllUsersExist(List<Guid> userIds, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0) return false;

        var tenantId = _currentUser.TenantId ?? "default";

        var existingUserCount = await _context.Users
            .Where(u => userIds.Contains(u.Id) && u.TenantId == tenantId)
            .CountAsync(cancellationToken);

        return existingUserCount == userIds.Count;
    }
}
