using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateOrganizationCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("组织 ID 不能为空")
            .MustAsync(OrganizationExists).WithMessage("组织不存在");

        RuleFor(x => x.Name)
            .Length(2, 200).WithMessage("组织名称长度必须在 2-200 个字符之间")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.FullName)
            .MaximumLength(500).WithMessage("完整名称长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.Type)
            .Must(BeValidType!).WithMessage("无效的组织类型")
            .When(x => !string.IsNullOrEmpty(x.Type));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("描述长度不能超过 1000 个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x)
            .MustAsync(ParentIsValid).WithMessage("不能将自己或子组织设为父组织")
            .When(x => x.ParentId.HasValue);

        // ✅ 修复：调整方法签名匹配 Guid?
        RuleFor(x => x.ManagerId)
            .MustAsync(async (managerId, cancellationToken) => await ManagerExists(managerId, cancellationToken))
            .WithMessage("负责人不存在")
            .When(x => x.ManagerId.HasValue);
    }

    private async Task<bool> OrganizationExists(Guid organizationId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Organizations
            .AnyAsync(o => o.Id == organizationId && o.TenantId == tenantId, cancellationToken);
    }

    private static bool BeValidType(string type)
    {
        return type == OrganizationType.Company
            || type == OrganizationType.Department
            || type == OrganizationType.Team;
    }

    private async Task<bool> ParentIsValid(UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        if (!command.ParentId.HasValue) return true;

        var tenantId = _currentUser.TenantId ?? "default";

        // 不能将自己设为父组织
        if (command.ParentId.Value == command.OrganizationId)
        {
            return false;
        }

        // 检查父组织是否存在
        var parent = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == command.ParentId.Value && o.TenantId == tenantId, cancellationToken);

        if (parent == null) return false;

        // 获取当前组织
        var current = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == command.OrganizationId, cancellationToken);

        if (current == null) return false;

        // 不能将子组织设为父组织（检查 parent.Path 是否包含 current.Path）
        if (parent.Path.StartsWith(current.Path))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> ManagerExists(Guid? managerId, CancellationToken cancellationToken)
    {
        if (!managerId.HasValue) return true;

        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Users
            .AnyAsync(u => u.Id == managerId.Value && u.TenantId == tenantId, cancellationToken);
    }
}
