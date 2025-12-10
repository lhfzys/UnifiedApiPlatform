using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Enums;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(ITokenService tokenService, IAuditLogService auditLogService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenService = tokenService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var ipAddress = request.IpAddress ?? "Unknown";
        try
        {
            var validationResult = await _tokenService.ValidateRefreshTokenAsync(
                request.RefreshToken, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("刷新令牌验证失败: {Reason}", validationResult.ErrorMessage);

                // 记录失败的刷新令牌日志
                await _auditLogService.LogLoginAsync(
                    validationResult.UserName ?? "Unknown",
                    LoginType.RefreshToken,
                    isSuccess: false,
                    ipAddress,
                    request.UserAgent,
                    failureReason: validationResult.ErrorMessage,
                    userId: validationResult.UserId, cancellationToken: cancellationToken);

                return Result.Fail<RefreshTokenResponse>(
                    validationResult.ErrorMessage ?? ErrorCodes.RefreshTokenInvalid);
            }

            var user = validationResult.User!;

            if (user.LockedUntil.HasValue && user.LockedUntil.Value > Instant.FromDateTimeUtc(DateTime.UtcNow))
            {
                _logger.LogWarning("刷新令牌失败：账户已锁定 - UserId: {UserId}", user.Id);

                await _auditLogService.LogLoginAsync(
                    user.UserName,
                    LoginType.RefreshToken,
                    isSuccess: false,
                    ipAddress,
                    request.UserAgent,
                    failureReason: "账户已锁定",
                    userId: user.Id, cancellationToken: cancellationToken);

                return Result.Fail<RefreshTokenResponse>("账户已锁定");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("刷新令牌失败：账户未激活 - UserId: {UserId}", user.Id);

                await _auditLogService.LogLoginAsync(
                    user.UserName,
                    LoginType.RefreshToken,
                    isSuccess: false,
                    ipAddress,
                    request.UserAgent,
                    failureReason: "账户未激活",
                    userId: user.Id, cancellationToken: cancellationToken);

                return Result.Fail<RefreshTokenResponse>(ErrorCodes.UserAccountInactive);
            }

            await _tokenService.RevokeRefreshTokenAsync(
                request.RefreshToken,
                ipAddress,
                "Replaced by new token",
                cancellationToken);

            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToList();

            var (accessToken, newRefreshToken, expiresAt) = await _tokenService.GenerateTokensAsync(
                user.Id,
                user.UserName,
                user.Email,
                user.TenantId,
                permissions,
                cancellationToken);

            await _auditLogService.LogLoginAsync(
                user.UserName,
                LoginType.RefreshToken,
                isSuccess: true,
                ipAddress,
                request.UserAgent,
                userId: user.Id, cancellationToken: cancellationToken);

            _logger.LogInformation("刷新令牌成功: UserId: {UserId}, UserName: {UserName}",
                user.Id, user.UserName);

            return Result.Ok(new RefreshTokenResponse
            {
                AccessToken = accessToken, RefreshToken = newRefreshToken, ExpiresAt = expiresAt.ToDateTimeUtc()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌异常");

            await _auditLogService.LogLoginAsync(
                "Unknown",
                LoginType.RefreshToken,
                isSuccess: false,
                ipAddress,
                request.UserAgent,
                failureReason: $"系统异常: {ex.Message}", cancellationToken: cancellationToken);

            return Result.Fail<RefreshTokenResponse>("刷新令牌失败");
        }
    }
}
