using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateUserCommandValidator(IApplicationDbContext context,ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .Length(3, 50).WithMessage("用户名长度必须在 3-50 个字符之间")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("用户名只能包含字母、数字和下划线")
            .MustAsync(BeUniqueUserName).WithMessage("用户名已存在");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(255).WithMessage("邮箱长度不能超过 255 个字符")
            .MustAsync(BeUniqueEmail).WithMessage("邮箱已被使用");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于 6 个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符")
            .Matches("[A-Z]").WithMessage("密码必须包含至少一个大写字母")
            .Matches("[a-z]").WithMessage("密码必须包含至少一个小写字母")
            .Matches("[0-9]").WithMessage("密码必须包含至少一个数字")
            .Matches("[^a-zA-Z0-9]").WithMessage("密码必须包含至少一个特殊字符");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{11}$").WithMessage("手机号格式不正确")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.OrganizationId)
            .MustAsync(OrganizationExists).WithMessage("组织不存在")
            .When(x => x.OrganizationId.HasValue);

        RuleFor(x => x.ManagerId)
            .MustAsync(ManagerExists).WithMessage("上级领导不存在")
            .When(x => x.ManagerId.HasValue);

        RuleFor(x => x.RoleIds)
            .Must(x => x.Count > 0).WithMessage("至少选择一个角色")
            .MustAsync(AllRolesExist).WithMessage("部分角色不存在");
    }

    private async Task<bool> BeUniqueUserName(CreateUserCommand command, string userName,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return !await _context.Users
            .AnyAsync(u => u.UserName == userName && u.TenantId == tenantId, cancellationToken);
    }

    private async Task<bool> BeUniqueEmail(CreateUserCommand command, string email, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";

        return !await _context.Users
            .AnyAsync(u => u.Email == email && u.TenantId == tenantId, cancellationToken);
    }

    private async Task<bool> OrganizationExists(Guid? organizationId, CancellationToken cancellationToken)
    {
        if (!organizationId.HasValue) return true;
        return await _context.Organizations.AnyAsync(o => o.Id == organizationId.Value, cancellationToken);
    }

    private async Task<bool> ManagerExists(Guid? managerId, CancellationToken cancellationToken)
    {
        if (!managerId.HasValue) return true;
        return await _context.Users.AnyAsync(u => u.Id == managerId.Value, cancellationToken);
    }

    private async Task<bool> AllRolesExist(List<Guid> roleIds, CancellationToken cancellationToken)
    {
        if (roleIds.Count == 0) return false;

        var existingRoleCount = await _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .CountAsync(cancellationToken);

        return existingRoleCount == roleIds.Count;
    }
}
