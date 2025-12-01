using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Domain.Enums;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IClock _clock;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IClock clock)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _clock = clock;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. 查找用户（包含租户信息）
        var user = await _context.Users
            .Include(u => u.Tenant)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            // 记录失败的登录日志
            await LogFailedLoginAsync(null, request, LoginStatus.Failed, "用户不存在", cancellationToken);
            return Result.Fail<LoginResponse>(ErrorCodes.UserInvalidCredentials);
        }

        // 2. 检查租户状态
        if (!user.Tenant.IsActive)
        {
            await LogFailedLoginAsync(user.Id, request, LoginStatus.Failed, "租户已被禁用", cancellationToken);
            return Result.Fail<LoginResponse>(ErrorCodes.TenantInactive);
        }

        // 3. 检查用户状态
        if (!user.IsActive)
        {
            await LogFailedLoginAsync(user.Id, request, LoginStatus.Failed, "用户账户未激活", cancellationToken);
            return Result.Fail<LoginResponse>(ErrorCodes.UserAccountInactive);
        }

        if (user.IsLocked(_clock))
        {
            await LogFailedLoginAsync(user.Id, request, LoginStatus.Blocked, "用户账户已锁定", cancellationToken);
            return Result.Fail<LoginResponse>(ErrorCodes.UserAccountLocked);
        }

        // 4. 验证密码
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordLoginFailure(_clock);
            await _context.SaveChangesAsync(cancellationToken);

            await LogFailedLoginAsync(user.Id, request, LoginStatus.Failed, "密码错误", cancellationToken);

            if (user.IsLocked(_clock))
            {
                return Result.Fail<LoginResponse>("密码错误次数过多，账户已被锁定15分钟");
            }

            return Result.Fail<LoginResponse>(ErrorCodes.UserInvalidCredentials);
        }

        // 5. 获取用户权限
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

        // 6. 生成 Token
        var tokenClaims = new TokenClaims
        {
            UserId = user.Id.ToString(),
            TenantId = user.TenantId,
            Email = user.Email,
            UserName = user.UserName,
            OrganizationId = user.OrganizationId?.ToString(),
            Roles = roles,
            Permissions = permissions
        };

        var accessToken = _tokenService.GenerateAccessToken(tokenClaims);
        var refreshTokenString = _tokenService.GenerateRefreshToken();

        // 7. 保存 RefreshToken
        await _refreshTokenService.CreateRefreshTokenAsync(
            user.Id,
            user.TenantId,
            refreshTokenString,
            request.IpAddress ?? "Unknown",
            cancellationToken);

        // 8. 更新用户登录信息
        user.RecordLoginSuccess(request.IpAddress ?? "Unknown", _clock);
        await _context.SaveChangesAsync(cancellationToken);

        // 9. 记录成功的登录日志
        await LogSuccessLoginAsync(user.Id, request, cancellationToken);

        // 10. 返回响应
        var response = new LoginResponse
        {
            Token = new TokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60), // 根据配置调整
                TokenType = "Bearer"
            },
            User = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Avatar = user.Avatar,
                Roles = roles,
                Permissions = permissions
            }
        };

        return Result.Ok(response);
    }

    private async Task LogSuccessLoginAsync(
        Guid userId,
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) return;

        var loginLog = new LoginLog
        {
            TenantId = user.TenantId,
            UserId = userId,
            UserName = request.Email,
            LoginType = "Password",
            Status = LoginStatus.Success,
            LoginAt = _clock.GetCurrentInstant()
        };

        _context.LoginLogs.Add(loginLog);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task LogFailedLoginAsync(
        Guid? userId,
        LoginCommand request,
        LoginStatus status,
        string reason,
        CancellationToken cancellationToken)
    {
        // 尝试获取租户ID（如果用户不存在，使用默认值）
        var tenantId = "unknown";
        if (userId.HasValue)
        {
            var user = await _context.Users.FindAsync(new object[] { userId.Value }, cancellationToken);
            if (user != null)
            {
                tenantId = user.TenantId;
            }
        }

        var loginLog = new LoginLog
        {
            TenantId = tenantId,
            UserId = userId,
            UserName = request.Email,
            LoginType = "Password",
            Status = status,
            FailureReason = reason,
            LoginAt = _clock.GetCurrentInstant()
        };

        _context.LoginLogs.Add(loginLog);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
