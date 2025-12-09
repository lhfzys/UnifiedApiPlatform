using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<UpdateRoleResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;

    public UpdateRoleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<UpdateRoleCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<UpdateRoleResponse>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 查询角色
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.TenantId == tenantId, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("角色不存在: {RoleId}", request.RoleId);
                return Result.Fail<UpdateRoleResponse>(ErrorCodes.RoleNotFound);
            }

            // 检查是否系统角色
            if (role.IsSystemRole)
            {
                _logger.LogWarning("尝试修改系统角色: {RoleId}", request.RoleId);
                return Result.Fail<UpdateRoleResponse>("不能修改系统角色");
            }

            // 乐观并发控制
            if (request.RowVersion != null && !role.RowVersion!.SequenceEqual(request.RowVersion))
            {
                _logger.LogWarning("角色数据已被修改: {RoleId}", request.RoleId);
                return Result.Fail<UpdateRoleResponse>("角色数据已被其他人修改，请刷新后重试");
            }

            // 更新字段（只更新提供的字段）
            if (!string.IsNullOrEmpty(request.DisplayName))
            {
                role.DisplayName = request.DisplayName;
            }

            if (request.Description != null)
            {
                role.Description = string.IsNullOrEmpty(request.Description) ? null : request.Description;
            }

            // 更新审计字段
            role.UpdatedAt = _clock.GetCurrentInstant();
            role.UpdatedBy = _currentUser.UserId;

            // 保存更改
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "角色更新成功: {RoleId}, 名称: {RoleName}, 操作人: {OperatorId}, IP: {IpAddress}",
                role.Id,
                role.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok(new UpdateRoleResponse { Id = role.Id, Name = role.Name, DisplayName = role.DisplayName });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "角色更新冲突: {RoleId}", request.RoleId);
            return Result.Fail<UpdateRoleResponse>("角色数据已被其他人修改，请刷新后重试");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "角色更新失败: {RoleId}", request.RoleId);
            return Result.Fail<UpdateRoleResponse>("更新角色失败");
        }
    }
}
