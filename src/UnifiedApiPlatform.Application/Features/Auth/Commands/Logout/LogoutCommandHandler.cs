using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        ITokenService tokenService,
        ICurrentUserService currentUser,
        ILogger<LogoutCommandHandler> logger)
    {
        _tokenService = tokenService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                await _tokenService.RevokeRefreshTokenAsync(
                    request.RefreshToken,
                    request.IpAddress ?? "Unknown",
                    "User logged out"
                );
            }

            _logger.LogInformation("用户登出成功: {UserId}", _currentUser.UserId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出失败");
            return Result.Fail("登出失败");
        }
    }
}
