using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _context = context;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<TokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. 验证刷新令牌
        var refreshToken = await _refreshTokenService.ValidateRefreshTokenAsync(
            request.RefreshToken,
            request.IpAddress ?? "Unknown",
            cancellationToken);

        if (refreshToken == null)
        {
            return Result.Fail<TokenResult>(ErrorCodes.RefreshTokenInvalid);
        }

        // 2. 获取用户信息（包含角色和权限）
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return Result.Fail<TokenResult>(ErrorCodes.UserNotFound);
        }

        // 3. 生成新的 Token
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

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
        var newRefreshTokenString = _tokenService.GenerateRefreshToken();

        // 4. 撤销旧的刷新令牌并创建新的
        await _refreshTokenService.RevokeRefreshTokenAsync(
            request.RefreshToken,
            request.IpAddress ?? "Unknown",
            "Replaced by new token",
            cancellationToken);

        await _refreshTokenService.CreateRefreshTokenAsync(
            user.Id,
            user.TenantId,
            newRefreshTokenString,
            request.IpAddress ?? "Unknown",
            cancellationToken);

        // 5. 返回新的令牌
        var result = new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            TokenType = "Bearer"
        };

        return Result.Ok(result);
    }
}
