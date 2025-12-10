using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        ITokenService tokenService,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService,
        ILogger<LogoutCommandHandler> logger)
    {
        _tokenService = tokenService;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = request.IpAddress ?? "Unknown";
        try
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var revokedCount = await _tokenService.RevokeAllUserTokensAsync(
                userId,
                ipAddress,
                "User logged out",
                cancellationToken);

            // 记录登出日志
            await _auditLogService.LogLoginAsync(
                _currentUser.UserName ?? "Unknown",
                LoginType.Logout,
                isSuccess: true,
                ipAddress,
                request.UserAgent,
                userId: userId, cancellationToken: cancellationToken);

            _logger.LogInformation("用户登出成功: {UserId}, UserName: {UserName}, 撤销 Token 数量: {Count}",
                userId, _currentUser.UserName, revokedCount);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出失败: {UserId}", _currentUser.UserId);

            await _auditLogService.LogLoginAsync(
                _currentUser.UserName ?? "Unknown",
                LoginType.Logout,
                isSuccess: false,
                ipAddress,
                request.UserAgent,
                failureReason: $"系统异常: {ex.Message}", cancellationToken: cancellationToken);
            return Result.Fail("登出失败");
        }
    }
}
