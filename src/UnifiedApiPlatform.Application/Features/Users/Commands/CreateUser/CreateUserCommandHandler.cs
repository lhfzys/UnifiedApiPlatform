using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<CreateUserCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";
            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PhoneNumber = request.PhoneNumber,
                Avatar = request.Avatar,
                Status = UserStatus.Active,
                IsActive = true,
                OrganizationId = request.OrganizationId,
                ManagerId = request.ManagerId,
                CreatedAt = _clock.GetCurrentInstant(),
                CreatedBy = _currentUser.UserId
            };

            _context.Users.Add(user);

            foreach (var roleId in request.RoleIds)
            {
                var userRole = new UserRole { UserId = user.Id, RoleId = roleId, };
                _context.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "用户创建成功: {UserId}, 用户名: {UserName}, 邮箱: {Email}, 操作人: {OperatorId}, IP: {IpAddress}",
                user.Id,
                user.UserName,
                user.Email,
                _currentUser.UserId,
                request.IpAddress
            );

            var response = new CreateUserResponse { Id = user.Id, UserName = user.UserName, Email = user.Email };

            return Result.Ok(response);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // unique_violation
            {
                var errorMessage = "创建失败";
                if (pgEx.ConstraintName?.Contains("email") == true)
                {
                    errorMessage = "邮箱已被使用";
                }
                else if (pgEx.ConstraintName?.Contains("username") == true)
                {
                    errorMessage = "用户名已存在";
                }

                _logger.LogWarning("创建用户失败（唯一性约束）: {UserName}, {Email}, 约束: {Constraint}",
                    request.UserName, request.Email, pgEx.ConstraintName);

                return Result.Fail<CreateUserResponse>(errorMessage);
            }

            _logger.LogError(ex, "创建用户失败（数据库错误）: {UserName}, {Email}", request.UserName, request.Email);
            return Result.Fail<CreateUserResponse>("创建用户失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建用户失败: {UserName}, {Email}", request.UserName, request.Email);
            return Result.Fail<CreateUserResponse>("创建用户失败");
        }
    }
}
