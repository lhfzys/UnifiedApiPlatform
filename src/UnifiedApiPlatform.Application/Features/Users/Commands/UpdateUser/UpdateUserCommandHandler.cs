using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.Id && u.TenantId == tenantId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("用户不存在: {UserId}", request.Id);
                return Result.Fail<UpdateUserResponse>(ErrorCodes.UserNotFound);
            }

            // 乐观并发控制
            if (request.RowVersion != null && !user.RowVersion.SequenceEqual(request.RowVersion))
            {
                _logger.LogWarning("用户数据已被修改: {UserId}", request.Id);
                return Result.Fail<UpdateUserResponse>("用户数据已被其他人修改，请刷新后重试");
            }

            if (!string.IsNullOrEmpty(request.UserName))
            {
                user.UserName = request.UserName;
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }

            if (request.PhoneNumber != null)
            {
                user.PhoneNumber = string.IsNullOrEmpty(request.PhoneNumber) ? null : request.PhoneNumber;
            }

            if (request.Avatar != null)
            {
                user.Avatar = string.IsNullOrEmpty(request.Avatar) ? null : request.Avatar;
            }

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                {
                    user.Activate();
                }
                else
                {
                    user.Deactivate();
                }
            }

            if (request.OrganizationId.HasValue)
            {
                user.OrganizationId = request.OrganizationId.Value;
            }

            if (request.ManagerId.HasValue)
            {
                user.ManagerId = request.ManagerId.Value;
            }

            // 更新审计字段
            user.UpdatedAt = _clock.GetCurrentInstant();
            user.UpdatedBy = _currentUser.UserId;

            if (request.RoleIds != null)
            {
                // 移除旧角色
                var existingRoles = user.UserRoles.ToList();
                foreach (var userRole in existingRoles)
                {
                    _context.UserRoles.Remove(userRole);
                }

                // 添加新角色
                foreach (var roleId in request.RoleIds)
                {
                    var userRole = new UserRole { UserId = user.Id, RoleId = roleId };
                    _context.UserRoles.Add(userRole);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "用户更新成功: {UserId}, 用户名: {UserName}, 操作人: {OperatorId}, IP: {IpAddress}",
                user.Id,
                user.UserName,
                _currentUser.UserId,
                request.IpAddress
            );

            var response = new UpdateUserResponse
            {
                Id = user.Id, UserName = user.UserName, Email = user.Email, IsActive = user.IsActive
            };

            return Result.Ok(response);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "用户更新冲突: {UserId}", request.Id);
            return Result.Fail<UpdateUserResponse>("用户数据已被其他人修改，请刷新后重试");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // unique_violation
            {
                string errorMessage = "更新失败";

                if (pgEx.ConstraintName?.Contains("email") == true)
                {
                    errorMessage = "邮箱已被使用";
                }
                else if (pgEx.ConstraintName?.Contains("username") == true)
                {
                    errorMessage = "用户名已存在";
                }

                _logger.LogWarning("用户更新失败（唯一性约束）: {UserId}, 约束: {Constraint}",
                    request.Id, pgEx.ConstraintName);

                return Result.Fail<UpdateUserResponse>(errorMessage);
            }

            _logger.LogError(ex, "用户更新失败（数据库错误）: {UserId}", request.Id);
            return Result.Fail<UpdateUserResponse>("更新用户失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用户更新失败: {UserId}", request.Id);
            return Result.Fail<UpdateUserResponse>("更新用户失败");
        }
    }
}
