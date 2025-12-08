using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("原密码不能为空")
            .When(x => IsChangingOwnPassword(x));

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("新密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于 6 个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符")
            .Matches("[A-Z]").WithMessage("密码必须包含至少一个大写字母")
            .Matches("[a-z]").WithMessage("密码必须包含至少一个小写字母")
            .Matches("[0-9]").WithMessage("密码必须包含至少一个数字")
            .Matches("[^a-zA-Z0-9]").WithMessage("密码必须包含至少一个特殊字符");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("确认密码不能为空")
            .Equal(x => x.NewPassword).WithMessage("两次输入的密码不一致");

        RuleFor(x => x.UserId)
            .MustAsync(UserExists).WithMessage("用户不存在")
            .When(x => x.UserId.HasValue);
    }

    private bool IsChangingOwnPassword(ChangePasswordCommand command)
    {
        return !command.UserId.HasValue ||
               command.UserId.ToString() == _currentUser.UserId;
    }

    private async Task<bool> UserExists(Guid? userId, CancellationToken cancellationToken)
    {
        if (!userId.HasValue) return true;

        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Users.AnyAsync(u => u.Id == userId.Value && u.TenantId == tenantId, cancellationToken);
    }
}
