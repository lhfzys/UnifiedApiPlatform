using FluentResults;
using MediatR;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ICurrentUserService _currentUser;

    public LogoutCommandHandler(
        IRefreshTokenService refreshTokenService,
        ICurrentUserService currentUser)
    {
        _refreshTokenService = refreshTokenService;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // 如果提供了 RefreshToken，撤销该令牌
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(
                request.RefreshToken,
                request.IpAddress ?? "Unknown",
                "User logout",
                cancellationToken);
        }
        // 否则撤销该用户的所有令牌
        else if (_currentUser.IsAuthenticated && Guid.TryParse(_currentUser.UserId, out var userId))
        {
            await _refreshTokenService.RevokeAllUserTokensAsync(
                userId,
                request.IpAddress ?? "Unknown",
                "User logout all",
                cancellationToken);
        }

        return Result.Ok();
    }
}
