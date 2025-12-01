using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Infrastructure.Options;
using UnifiedApiPlatform.Infrastructure.Persistence;
using UnifiedApiPlatform.Shared.Extensions;

namespace UnifiedApiPlatform.Infrastructure.Identity.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly IClock _clock;

    public RefreshTokenService(
        ApplicationDbContext context,
        IOptions<JwtSettings> jwtSettings,
        IClock clock)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _clock = clock;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(
        Guid userId,
        string tenantId,
        string token,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var now = _clock.GetCurrentInstant();
        var expiresAt = now.Plus(Duration.FromDays(_jwtSettings.RefreshTokenExpirationDays));

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TenantId = tenantId,
            Token = token.ToSha256(), // 存储哈希值
            ExpiresAt = expiresAt,
            CreatedAt = now,
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(refreshToken);

        // 限制用户的刷新令牌数量
        await RemoveOldRefreshTokensAsync(userId, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(
        string token,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = token.ToSha256();

        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash, cancellationToken);

        if (refreshToken == null)
            return null;

        // 检查是否已撤销
        if (refreshToken.IsRevoked())
            return null;

        // 检查是否过期
        if (refreshToken.IsExpired(_clock))
            return null;

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(
        string token,
        string ipAddress,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = token.ToSha256();
        var now = _clock.GetCurrentInstant();

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash, cancellationToken);

        if (refreshToken == null || refreshToken.IsRevoked())
            return;

        refreshToken.RevokedAt = now;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.RevokeReason = reason;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(
        Guid userId,
        string ipAddress,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var now = _clock.GetCurrentInstant();

        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
            token.RevokedByIp = ipAddress;
            token.RevokeReason = reason;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = _clock.GetCurrentInstant();
        var thirtyDaysAgo = now.Minus(Duration.FromDays(30));

        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < thirtyDaysAgo)
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(expiredTokens);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task RemoveOldRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);

        // 保留最新的 N 个令牌
        var maxTokens = _jwtSettings.MaxRefreshTokensPerUser;
        if (userTokens.Count > maxTokens)
        {
            var tokensToRemove = userTokens.Skip(maxTokens).ToList();
            _context.RefreshTokens.RemoveRange(tokensToRemove);
        }
    }
}
