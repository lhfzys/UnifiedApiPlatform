using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<CreateRoleResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    public CreateRoleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<CreateRoleResponse>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";
            var now = _clock.GetCurrentInstant();

            var role = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description,
                IsSystemRole = false,
                CreatedAt = now,
                CreatedBy = _currentUser.UserId
            };

            _context.Roles.Add(role);

            if (request.PermissionCodes != null && request.PermissionCodes.Count > 0)
            {
                foreach (var permissionCode in request.PermissionCodes)
                {
                    var rolePermission = new RolePermission { RoleId = role.Id, PermissionId = new Guid(permissionCode) };
                    _context.RolePermissions.Add(rolePermission);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "角色创建成功: {RoleId}, 名称: {RoleName}, 操作人: {OperatorId}, IP: {IpAddress}",
                role.Id,
                role.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok(new CreateRoleResponse { Id = role.Id, Name = role.Name, DisplayName = role.DisplayName });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // unique_violation
            {
                string errorMessage = "创建角色失败";

                if (pgEx.ConstraintName?.Contains("name") == true)
                {
                    errorMessage = "角色名称已存在";
                }

                _logger.LogWarning("角色创建失败（唯一性约束）: {Name}, 约束: {Constraint}",
                    request.Name, pgEx.ConstraintName);

                return Result.Fail<CreateRoleResponse>(errorMessage);
            }

            _logger.LogError(ex, "角色创建失败（数据库错误）: {Name}", request.Name);
            return Result.Fail<CreateRoleResponse>("创建角色失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "角色创建失败: {Name}", request.Name);
            return Result.Fail<CreateRoleResponse>("创建角色失败");
        }
    }
}
