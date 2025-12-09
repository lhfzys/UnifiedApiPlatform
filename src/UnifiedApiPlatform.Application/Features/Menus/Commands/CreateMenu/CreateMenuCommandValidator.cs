using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.CreateMenu;

public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateMenuCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("菜单编码不能为空")
            .Length(2, 50).WithMessage("菜单编码长度必须在 2-50 个字符之间")
            .Matches("^[a-zA-Z][a-zA-Z0-9_-]*$").WithMessage("菜单编码必须以字母开头，只能包含字母、数字、下划线和连字符")
            .MustAsync(BeUniqueCode).WithMessage("菜单编码已存在");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("菜单名称不能为空")
            .Length(2, 100).WithMessage("菜单名称长度必须在 2-100 个字符之间");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("无效的菜单类型");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("图标长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.Icon));

        RuleFor(x => x.Path)
            .MaximumLength(200).WithMessage("路由路径长度不能超过 200 个字符")
            .When(x => !string.IsNullOrEmpty(x.Path));

        RuleFor(x => x.Component)
            .MaximumLength(200).WithMessage("组件路径长度不能超过 200 个字符")
            .When(x => !string.IsNullOrEmpty(x.Component));

        RuleFor(x => x.PermissionCode)
            .MaximumLength(100).WithMessage("权限代码长度不能超过 100 个字符")
            .MustAsync(async (permissionCode, cancellationToken) =>
                await PermissionExists(permissionCode, cancellationToken))
            .WithMessage("权限不存在")
            .When(x => !string.IsNullOrEmpty(x.PermissionCode));

        RuleFor(x => x.ParentId)
            .MustAsync(async (parentId, cancellationToken) =>
                await ParentExists(parentId, cancellationToken))
            .WithMessage("父菜单不存在")
            .When(x => x.ParentId.HasValue);
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";

        return !await _context.Menus
            .AnyAsync(m => m.Code == code && (m.TenantId == tenantId || m.TenantId == null),
                cancellationToken);
    }

    private async Task<bool> PermissionExists(string? permissionCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(permissionCode)) return true;

        var tenantId = _currentUser.TenantId ?? "default";

        return await _context.Permissions
            .AnyAsync(p => p.Code == permissionCode && p.TenantId == tenantId,
                cancellationToken);
    }

    private async Task<bool> ParentExists(Guid? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;

        var tenantId = _currentUser.TenantId ?? "default";

        return await _context.Menus
            .AnyAsync(m => m.Id == parentId.Value && (m.TenantId == tenantId || m.TenantId == null),
                cancellationToken);
    }
}
