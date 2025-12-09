using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.AssignPermissions;

public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AssignPermissionsCommandHandler> _logger;

    public AssignPermissionsCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<AssignPermissionsCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }


    public async Task<Result> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 查询角色
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.TenantId == tenantId, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail(ErrorCodes.RoleNotFound);
            }

            // 移除所有现有权限
            var existingPermissions = role.RolePermissions.ToList();
            foreach (var rolePermission in existingPermissions)
            {
                _context.RolePermissions.Remove(rolePermission);
            }

            // 添加新权限
            foreach (var permissionCode in request.PermissionCodes)
            {
                var rolePermission = new RolePermission { RoleId = role.Id, PermissionCode = permissionCode };
                _context.RolePermissions.Add(rolePermission);
            }

            // 保存更改
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "角色权限分配成功: {RoleId}, 权限数量: {PermissionCount}, 操作人: {OperatorId}, IP: {IpAddress}",
                role.Id,
                request.PermissionCodes.Count,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分配权限失败: {RoleId}", request.RoleId);
            return Result.Fail("分配权限失败");
        }
    }
}
