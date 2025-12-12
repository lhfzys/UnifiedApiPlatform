using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, Result<TenantDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTenantByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TenantDetailDto>> Handle(
        GetTenantByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants
            .Where(t => t.Id.ToString() == request.Id && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null)
        {
            return Result.Fail<TenantDetailDto>(ErrorCodes.TenantNotFound);
        }

        // 统计信息
        var totalUsers = await _context.Users
            .CountAsync(u => u.TenantId == tenant.Id.ToString() && !u.IsDeleted, cancellationToken);

        var activeUsers = await _context.Users
            .CountAsync(u => u.TenantId == tenant.Id.ToString() && u.IsActive && !u.IsDeleted, cancellationToken);

        var totalRoles = await _context.Roles
            .CountAsync(r => r.TenantId == tenant.Id.ToString() && !r.IsDeleted, cancellationToken);

        var totalOrganizations = await _context.Organizations
            .CountAsync(o => o.TenantId == tenant.Id.ToString() && !o.IsDeleted, cancellationToken);

        var totalMenus = await _context.Menus
            .CountAsync(m => m.TenantId == tenant.Id.ToString() && !m.IsDeleted, cancellationToken);

        // 最后登录时间
        var lastLoginTime = await _context.LoginLogs
            .Where(l => l.TenantId == tenant.Id.ToString() && l.IsSuccess)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => l.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var result = new TenantDetailDto
        {
            Id = tenant.Id.ToString(),
            Identifier = tenant.Identifier,
            Name = tenant.Name,
            Description = tenant.Description,
            ContactEmail = tenant.ContactEmail,
            ContactPhone = tenant.ContactPhone,
            Address = tenant.Address,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt.ToDateTimeUtc(),
            CreatedBy = tenant.CreatedBy,
            UpdatedAt = tenant.UpdatedAt?.ToDateTimeUtc(),
            UpdatedBy = tenant.UpdatedBy,
            Statistics = new TenantDetailStatistics
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalRoles = totalRoles,
                TotalOrganizations = totalOrganizations,
                TotalMenus = totalMenus,
                LastLoginTime = lastLoginTime != default
                    ? lastLoginTime.ToDateTimeUtc()
                    : null
            }
        };

        return Result.Ok(result);
    }
}
