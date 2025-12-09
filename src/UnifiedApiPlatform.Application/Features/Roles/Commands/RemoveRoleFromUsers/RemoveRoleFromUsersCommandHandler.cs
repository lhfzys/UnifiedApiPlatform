using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.RemoveRoleFromUsers;

public class RemoveRoleFromUsersCommandHandler : IRequestHandler<RemoveRoleFromUsersCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<RemoveRoleFromUsersCommandHandler> _logger;

    public RemoveRoleFromUsersCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<RemoveRoleFromUsersCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveRoleFromUsersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 验证角色存在
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.TenantId == tenantId, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail(ErrorCodes.RoleNotFound);
            }

            // 查找需要删除的用户角色关联
            var userRolesToRemove = await _context.UserRoles
                .Where(ur => ur.RoleId == request.RoleId && request.UserIds.Contains(ur.UserId))
                .ToListAsync(cancellationToken);

            if (userRolesToRemove.Count > 0)
            {
                _context.UserRoles.RemoveRange(userRolesToRemove);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "移除用户角色成功: {RoleId}, 移除用户数: {Count}, 操作人: {OperatorId}, IP: {IpAddress}",
                    request.RoleId,
                    userRolesToRemove.Count,
                    _currentUser.UserId,
                    request.IpAddress
                );
            }
            else
            {
                _logger.LogInformation("没有找到需要移除的用户角色关联: {RoleId}", request.RoleId);
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除用户角色失败: {RoleId}", request.RoleId);
            return Result.Fail("移除用户角色失败");
        }
    }
}
