using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Domain.Enums;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IClock _clock;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IClock clock,
        IAuditLogService auditLogService,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _clock = clock;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = request.IpAddress ?? "Unknown";
        try
        {
            // 查询用户
            var user = await FindUserAsync(request.Account, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("登录失败：用户不存在 - {Account}", request.Account);

                // 记录失败的登录日志
                await _auditLogService.LogLoginAsync(
                    request.Account,
                    LoginType.Login,
                    isSuccess: false,
                    ipAddress,
                    request.UserAgent,
                    failureReason: "用户不存在", cancellationToken: cancellationToken);

                return Result.Fail<LoginResponse>(ErrorCodes.UserInvalidCredentials);
            }

            if (user.IsLocked)
            {
                var lockUntil = user.LockedUntil?.ToDateTimeUtc();
                var remainingMinutes = lockUntil.HasValue
                    ? Math.Max(0, (int)(lockUntil.Value - DateTime.UtcNow).TotalMinutes)
                    : 0;
                _logger.LogWarning("登录失败：账户已锁定 - {Account}, 解锁时间: {LockUntil}",
                    request.Account, lockUntil);

                await _auditLogService.LogLoginAsync(
                    user.UserName,
                    LoginType.Login,
                    isSuccess: false,
                    ipAddress,
                    request.UserAgent,
                    failureReason: $"账户已锁定，剩余 {remainingMinutes} 分钟",
                    userId: user.Id, cancellationToken: cancellationToken);

                return Result.Fail<LoginResponse>(
                    $"账户已锁定，请 {remainingMinutes} 分钟后再试");
            }

            // 验证密码
            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("登录失败：密码错误 - {Account}, 失败次数: {Count}",
                    request.Account, user.LoginFailureCount + 1);

                user.RecordLoginFailure(_clock);
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogLoginAsync(
                    user.UserName,
                    LoginType.Login,
                    isSuccess: false,
                    ipAddress,
                    request.UserAgent,
                    failureReason: user.IsLocked
                        ? $"密码错误，账户已锁定 15 分钟"
                        : $"密码错误，剩余尝试次数: {5 - user.LoginFailureCount}",
                    userId: user.Id, cancellationToken: cancellationToken);

                return Result.Fail<LoginResponse>(
                    user.IsLocked
                        ? "密码错误次数过多，账户已锁定 15 分钟"
                        : ErrorCodes.UserInvalidCredentials);
            }

            // 检查账户状态
            if (!user.IsActive)
            {
                _logger.LogWarning("登录失败：账户未激活 - {Account}", request.Account);

                await _auditLogService.LogLoginAsync(
                    user.UserName,
                    LoginType.Login,
                    isSuccess: false,
                    ipAddress,
                    request.UserAgent,
                    failureReason: "账户未激活",
                    userId: user.Id, cancellationToken: cancellationToken);

                return Result.Fail<LoginResponse>(ErrorCodes.UserAccountInactive);
            }


            await CheckAbnormalLocationAsync(user, ipAddress);

            // 获取用户权限
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToList();
            var (accessToken, refreshToken, expiresAt) = await _tokenService.GenerateTokensAsync(
                user.Id,
                user.UserName,
                user.Email,
                user.TenantId,
                permissions,
                cancellationToken);

            var refreshTokenHash = ComputeSha256Hash(refreshToken);
            var refreshTokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenHash, cancellationToken);

            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.CreatedByIp = ipAddress;
            }

            user.RecordLoginSuccess(ipAddress, _clock);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogLoginAsync(
                user.UserName,
                LoginType.Login,
                isSuccess: true,
                ipAddress,
                request.UserAgent,
                userId: user.Id, cancellationToken: cancellationToken);

            _logger.LogInformation("用户登录成功: {UserName}, IP: {IpAddress}",
                user.UserName, ipAddress);

            return Result.Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt.ToDateTimeUtc(),
                User = new LoginUserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    TenantId = user.TenantId
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录处理异常: {Account}", request.Account);

            await _auditLogService.LogLoginAsync(
                request.Account,
                LoginType.Login,
                isSuccess: false,
                ipAddress,
                request.UserAgent,
                failureReason: $"系统异常: {ex.Message}", cancellationToken: cancellationToken);

            return Result.Fail<LoginResponse>("登录失败，请稍后重试");
        }
    }

    /// <summary>
    /// 查找用户（支持用户名或邮箱）
    /// </summary>
    private async Task<User?> FindUserAsync(string account, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u =>
                    u.UserName == account || u.Email == account,
                cancellationToken);
    }

    /// <summary>
    /// 检查异地登录
    /// </summary>
    private async Task CheckAbnormalLocationAsync(User user, string ipAddress)
    {
        try
        {
            // 查询最近一次成功登录
            var lastLogin = await _context.LoginLogs
                .Where(l => l.UserId == user.Id &&
                            l.IsSuccess &&
                            l.LoginType == LoginType.Login)
                .OrderByDescending(l => l.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastLogin == null || string.IsNullOrEmpty(lastLogin.Location))
                return;

            // 获取当前登录地理位置
            var currentLocation = await Task.Run(() =>
            {
                var locationService = _context as IApplicationDbContext;
                // 需要通过服务定位器或其他方式获取 IIpLocationService
                // 这里简化处理，实际应该注入服务
                return (string?)null;
            });

            if (string.IsNullOrEmpty(currentLocation))
                return;

            // 如果地理位置不同，记录警告
            if (lastLogin.Location != currentLocation)
            {
                _logger.LogWarning(
                    "检测到异地登录: {UserName}, 上次: {LastLocation} ({LastIp}), 本次: {CurrentLocation} ({CurrentIp})",
                    user.UserName, lastLogin.Location, lastLogin.IpAddress,
                    currentLocation, ipAddress);

                // TODO: 发送异地登录通知
                // await _notificationService.SendAbnormalLoginNotificationAsync(user, lastLogin, currentLocation, ipAddress);
            }
        }
        catch (Exception ex)
        {
            // 异地登录检测失败不应该影响登录流程
            _logger.LogError(ex, "检测异地登录失败: {UserName}", user.UserName);
        }
    }

    /// <summary>
    /// 计算 SHA256 哈希
    /// </summary>
    private static string ComputeSha256Hash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
