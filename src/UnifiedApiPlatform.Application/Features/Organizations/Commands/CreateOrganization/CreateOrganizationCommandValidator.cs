using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateOrganizationCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("组织编码不能为空")
            .Length(2, 50).WithMessage("组织编码长度必须在 2-50 个字符之间")
            .Matches("^[a-zA-Z][a-zA-Z0-9_]*$").WithMessage("组织编码必须以字母开头，只能包含字母、数字和下划线")
            .MustAsync(BeUniqueCode).WithMessage("组织编码已存在");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("组织名称不能为空")
            .Length(2, 200).WithMessage("组织名称长度必须在 2-200 个字符之间");

        RuleFor(x => x.FullName)
            .MaximumLength(500).WithMessage("完整名称长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("组织类型不能为空")
            .Must(BeValidType).WithMessage("无效的组织类型");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("描述长度不能超过 1000 个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ParentId)
            .MustAsync(async (parentId, cancellationToken) => await ParentExists(parentId, cancellationToken))
            .WithMessage("父组织不存在")
            .When(x => x.ParentId.HasValue);

        RuleFor(x => x.ManagerId)
            .MustAsync(async (managerId, cancellationToken) => await ManagerExists(managerId, cancellationToken))
            .WithMessage("负责人不存在")
            .When(x => x.ManagerId.HasValue);
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return !await _context.Organizations
            .AnyAsync(o => o.Code == code && o.TenantId == tenantId, cancellationToken);
    }

    private static bool BeValidType(string type)
    {
        return type == OrganizationType.Company
            || type == OrganizationType.Department
            || type == OrganizationType.Team;
    }

    // ✅ 修复：参数类型改为 Guid?
    private async Task<bool> ParentExists(Guid? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;

        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Organizations
            .AnyAsync(o => o.Id == parentId.Value && o.TenantId == tenantId, cancellationToken);
    }

    // ✅ 修复：参数类型改为 Guid?
    private async Task<bool> ManagerExists(Guid? managerId, CancellationToken cancellationToken)
    {
        if (!managerId.HasValue) return true;

        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Users
            .AnyAsync(u => u.Id == managerId.Value && u.TenantId == tenantId, cancellationToken);
    }
}
