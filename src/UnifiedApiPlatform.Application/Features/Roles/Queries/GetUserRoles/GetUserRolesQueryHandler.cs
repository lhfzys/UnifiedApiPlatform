using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<List<UserRoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUserRolesQueryHandler> _logger;

    public GetUserRolesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetUserRolesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<UserRoleDto>>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
// 验证用户存在
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.UserId && u.TenantId == request.CurrentTenantId, cancellationToken);

            if (!userExists)
            {
                _logger.LogWarning("用户不存在: {UserId}", request.UserId);
                return Result.Fail<List<UserRoleDto>>(ErrorCodes.UserNotFound);
            }

            // 查询用户的所有角色
            var roles = await _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == request.UserId)
                .Select(ur => new UserRoleDto
                {
                    RoleId = ur.Role.Id,
                    Name = ur.Role.Name,
                    DisplayName = ur.Role.DisplayName,
                    Description = ur.Role.Description,
                    IsSystemRole = ur.Role.IsSystemRole,
                    AssignedAt = ur.CreatedAt.ToDateTimeUtc()
                })
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);

            return Result.Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户角色列表失败: {UserId}", request.UserId);
            return Result.Fail<List<UserRoleDto>>("获取用户角色列表失败");
        }
    }
}
