using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserCommandValidator(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("用户 ID 不能为空");

        RuleFor(x => x.UserName)
            .Length(3, 50).WithMessage("用户名长度必须在 3-50 个字符之间")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("用户名只能包含字母、数字和下划线")
            .MustAsync(BeUniqueUserName).WithMessage("用户名已存在")
            .When(x => !string.IsNullOrEmpty(x.UserName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(255).WithMessage("邮箱长度不能超过 255 个字符")
            .MustAsync(BeUniqueEmail).WithMessage("邮箱已被使用")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{11}$").WithMessage("手机号格式不正确")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.OrganizationId)
            .MustAsync(OrganizationExists).WithMessage("组织不存在")
            .When(x => x.OrganizationId.HasValue);

        RuleFor(x => x.ManagerId)
            .MustAsync(ManagerExists).WithMessage("上级领导不存在")
            .Must((command, managerId) => managerId != command.Id).WithMessage("不能将自己设为上级领导")
            .When(x => x.ManagerId.HasValue);

        RuleFor(x => x.RoleIds)
            .Must(x => x!.Count > 0).WithMessage("至少选择一个角色")
            .MustAsync(AllRolesExist!).WithMessage("部分角色不存在")
            .When(x => x.RoleIds != null);
    }

    private async Task<bool> BeUniqueUserName(UpdateUserCommand command, string? userName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userName)) return true;

        var tenantId = _currentUser.TenantId ?? "default";

        return !await _context.Users
            .AnyAsync(u => u.Id != command.Id &&
                           u.UserName == userName &&
                           u.TenantId == tenantId,
                cancellationToken);
    }

    private async Task<bool> BeUniqueEmail(UpdateUserCommand command, string? email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email))
        {
            return true;
        }

        var tenantId = _currentUser.TenantId ?? "default";

        return !await _context.Users
            .AnyAsync(u => u.Id != command.Id &&
                           u.Email == email &&
                           u.TenantId == tenantId,
                cancellationToken);
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
