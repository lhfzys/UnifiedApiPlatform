using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    public DeleteRoleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<DeleteRoleCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 查询角色（包含关联数据）
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .Include(r => r.RoleMenus)
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.TenantId == tenantId, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail(ErrorCodes.RoleNotFound);
            }

            // 软删除角色
            role.IsDeleted = true;
            role.DeletedAt = _clock.GetCurrentInstant();
            role.DeletedBy = _currentUser.UserId;

            // 删除角色权限关联
            foreach (var rolePermission in role.RolePermissions.ToList())
            {
                _context.RolePermissions.Remove(rolePermission);
            }

            // 删除角色菜单关联
            foreach (var roleMenu in role.RoleMenus.ToList())
            {
                _context.RoleMenus.Remove(roleMenu);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "角色删除成功: {RoleId}, 名称: {RoleName}, 操作人: {OperatorId}, IP: {IpAddress}",
                role.Id,
                role.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除角色失败: {RoleId}", request.RoleId);
            return Result.Fail("删除角色失败");
        }
    }
}
