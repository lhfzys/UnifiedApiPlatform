using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IClock clock, ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 查询用户
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("登录失败: 用户不存在 {Email}", request.Email);
                return Result.Fail<LoginResponse>(ErrorCodes.UserInvalidCredentials);
            }

            // 验证密码
            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                // 记录失败次数
                user.RecordLoginFailure(_clock);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogWarning("登录失败: 密码错误 {Email}", request.Email);
                return Result.Fail<LoginResponse>(ErrorCodes.UserInvalidCredentials);
            }

            // 检查账户状态
            if (!user.IsActive)
            {
                _logger.LogWarning("登录失败: 账户未激活 {Email}", request.Email);
                return Result.Fail<LoginResponse>(ErrorCodes.UserAccountInactive);
            }

            if (user.IsLocked(_clock))
            {
                _logger.LogWarning("登录失败: 账户已锁定 {Email}", request.Email);
                return Result.Fail<LoginResponse>(ErrorCodes.UserAccountLocked);
            }

            // ✅ 准备 Token Claims
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.PermissionCode)
                .Distinct()
                .ToList();

            var tokenClaims = new TokenClaims
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                TenantId = user.TenantId,
                OrganizationId = user.OrganizationId?.ToString(),
                Roles = roles,
                Permissions = permissions
            };

            var accessToken = _tokenService.GenerateAccessToken(tokenClaims);

            var deviceInfo = $"{request.UserAgent} - {request.IpAddress}";
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(
                user.Id,
                user.TenantId,
                request.IpAddress ?? "Unknown",
                deviceInfo
            );

            user.RecordLoginSuccess(request.IpAddress, _clock);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("用户登录成功: {Email}, IP: {IpAddress}", user.Email, request.IpAddress);

            return Result.Ok(new LoginResponse
            {
                Token = new TokenResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = refreshToken.ExpiresAt.ToDateTimeUtc(),
                    TokenType = "Bearer"
                },
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Avatar = user.Avatar
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录处理失败: {Email}", request.Email);
            return Result.Fail<LoginResponse>("登录失败，请稍后重试");
        }
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
            IpAddress = request.IpAddress ?? "Unknown",
            UserAgent = request.UserAgent ?? "Unknown",
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
            IpAddress = request.IpAddress ?? "Unknown",
            UserAgent = request.UserAgent ?? "Unknown",
            LoginType = "Password",
            Status = status,
            FailureReason = reason,
            LoginAt = _clock.GetCurrentInstant()
        };

        _context.LoginLogs.Add(loginLog);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
