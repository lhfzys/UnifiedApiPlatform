using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.AssignRolesToUsers;

public class AssignRolesToUsersCommandHandler : IRequestHandler<AssignRolesToUsersCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AssignRolesToUsersCommandHandler> _logger;

    public AssignRolesToUsersCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<AssignRolesToUsersCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result> Handle(AssignRolesToUsersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.TenantId == tenantId, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail(ErrorCodes.RoleNotFound);
            }

            var existingUserRoles = await _context.UserRoles
                .Where(ur => ur.RoleId == request.RoleId && request.UserIds.Contains(ur.UserId))
                .Select(ur => ur.UserId)
                .ToListAsync(cancellationToken);

            var newUserIds = request.UserIds.Except(existingUserRoles).ToList();

            if (newUserIds.Count > 0)
            {
                foreach (var userId in newUserIds)
                {
                    var userRole = new UserRole { UserId = userId, RoleId = request.RoleId };
                    _context.UserRoles.Add(userRole);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "批量分配角色成功: {RoleId}, 新增用户数: {Count}, 操作人: {OperatorId}, IP: {IpAddress}",
                    request.RoleId,
                    newUserIds.Count,
                    _currentUser.UserId,
                    request.IpAddress
                );
            }
            else
            {
                _logger.LogInformation("所有用户已拥有该角色: {RoleId}", request.RoleId);
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量分配角色失败: {RoleId}", request.RoleId);
            return Result.Fail("批量分配角色失败");
        }
    }
}
