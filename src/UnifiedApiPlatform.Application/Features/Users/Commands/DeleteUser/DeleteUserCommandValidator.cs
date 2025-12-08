using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteUserCommandValidator(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户 ID 不能为空")
            .MustAsync(UserExists).WithMessage("用户不存在")
            .MustAsync(NotDeletingSelf).WithMessage("不能删除自己")
            .MustAsync(NotSystemUser).WithMessage("不能删除系统用户");
    }

    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Users.AnyAsync(u => u.Id == userId && u.TenantId == tenantId, cancellationToken);
    }

    private Task<bool> NotDeletingSelf(Guid userId, CancellationToken cancellationToken)
    {
        // 不能删除自己
        var currentUserId = _currentUser.UserId;
        return Task.FromResult(userId.ToString() != currentUserId);
    }

    private async Task<bool> NotSystemUser(Guid userId, CancellationToken cancellationToken)
    {
        // 检查是否是系统用户（拥有 SuperAdmin 角色）
        var hasSystemRole = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .AnyAsync(ur => ur.Role.Name == "SuperAdmin" && ur.Role.IsSystemRole, cancellationToken);

        return !hasSystemRole;
    }
}
