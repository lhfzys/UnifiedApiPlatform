using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Features.Permissions.Queries.GetPermissions;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRoleByIdQueryHandler> _logger;

    public GetRoleByIdQueryHandler(IApplicationDbContext context, ILogger<GetRoleByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _context.Roles
                .AsNoTracking()
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.TenantId == request.CurrentTenantId,
                    cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail<RoleDetailDto>(ErrorCodes.RoleNotFound);
            }

            var dto = new RoleDetailDto
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.DisplayName,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                UserCount = role.UserRoles.Count,
                Permissions = role.RolePermissions
                    .Select(rp => new PermissionDto
                    {
                        Id = rp.Permission.Id,
                        Code = rp.Permission.Code,
                        Name = rp.Permission.Name,
                        Category = rp.Permission.Category,
                        Description = rp.Permission.Description,
                        IsSystemPermission = rp.Permission.IsSystemPermission
                    })
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Code)
                    .ToList(),
                CreatedAt = role.CreatedAt.ToDateTimeUtc(),
                CreatedBy = role.CreatedBy,
                UpdatedAt = role.UpdatedAt?.ToDateTimeUtc(),
                UpdatedBy = role.UpdatedBy,
                RowVersion = role.RowVersion != null ? Convert.ToBase64String(role.RowVersion) : null
            };

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色详情失败: {RoleId}", request.RoleId);
            return Result.Fail<RoleDetailDto>("获取角色详情失败");
        }
    }
}
