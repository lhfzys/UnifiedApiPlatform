using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandValidator : AbstractValidator<DeleteOrganizationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteOrganizationCommandValidator(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("组织 ID 不能为空")
            .MustAsync(OrganizationExists).WithMessage("组织不存在")
            .MustAsync(HasNoChildren).WithMessage("组织存在子组织，不能删除")
            .MustAsync(HasNoUsers).WithMessage("组织存在用户，不能删除");
    }

    private async Task<bool> OrganizationExists(Guid organizationId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? "default";
        return await _context.Organizations
            .AnyAsync(o => o.Id == organizationId && o.TenantId == tenantId, cancellationToken);
    }

    private async Task<bool> HasNoChildren(Guid organizationId, CancellationToken cancellationToken)
    {
        var hasChildren = await _context.Organizations
            .AnyAsync(o => o.ParentId == organizationId, cancellationToken);

        return !hasChildren;
    }

    private async Task<bool> HasNoUsers(Guid organizationId, CancellationToken cancellationToken)
    {
        var hasUsers = await _context.Users
            .AnyAsync(u => u.OrganizationId == organizationId, cancellationToken);

        return !hasUsers;
    }
}
