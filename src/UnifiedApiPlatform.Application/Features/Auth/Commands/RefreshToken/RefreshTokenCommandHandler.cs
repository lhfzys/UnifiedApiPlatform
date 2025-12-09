using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<TokenResult>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // ✅ 验证刷新令牌
            var oldRefreshToken = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);

            if (oldRefreshToken == null)
            {
                _logger.LogWarning("刷新令牌无效或已过期");
                return Result.Fail<TokenResult>(ErrorCodes.RefreshTokenInvalid);
            }

            // 加载用户及其角色权限
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                .FirstOrDefaultAsync(u => u.Id == oldRefreshToken.UserId, cancellationToken);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("用户不存在或未激活: {UserId}", oldRefreshToken.UserId);
                return Result.Fail<TokenResult>(ErrorCodes.UserNotFound);
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

            // ✅ 生成新的 Access Token
            var newAccessToken = _tokenService.GenerateAccessToken(tokenClaims);

            // ✅ 生成新的 Refresh Token
            var deviceInfo = oldRefreshToken.DeviceInfo;
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(
                user.Id,
                user.TenantId,
                request.IpAddress ?? "Unknown",
                deviceInfo
            );

            // ✅ 撤销旧的 Refresh Token（令牌轮换）
            await _tokenService.RevokeRefreshTokenAsync(
                oldRefreshToken.Token,
                request.IpAddress ?? "Unknown",
                "Replaced by new refresh token"
            );

            // 可选：将旧令牌标记为被替换
            oldRefreshToken.ReplacedByToken = newRefreshToken.Token;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("令牌刷新成功: {UserId}", user.Id);

            // ✅ 返回新令牌
            return Result.Ok(new TokenResult
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = newRefreshToken.ExpiresAt.ToDateTimeUtc(),
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌失败");
            return Result.Fail<TokenResult>("刷新令牌失败，请重新登录");
        }
    }
}
