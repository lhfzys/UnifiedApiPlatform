using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Infrastructure.Options;
using UnifiedApiPlatform.Infrastructure.Persistence;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Infrastructure.Identity.Services;

public class JwtTokenService : ITokenService
{
   private readonly JwtSettings _jwtSettings;
    private readonly ApplicationDbContext _context;
    private readonly IClock _clock;

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        ApplicationDbContext context,
        IClock clock)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
        _clock = clock;
    }

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    public string GenerateAccessToken(TokenClaims claims)
    {
        var tokenClaims = new List<Claim>
        {
            // 标准 Claims
            new(JwtRegisteredClaimNames.Sub, claims.UserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, claims.Email),

            // 自定义 Claims
            new(CustomClaimTypes.UserId, claims.UserId),
            new(CustomClaimTypes.UserName, claims.UserName),
            new(CustomClaimTypes.Email, claims.Email),
            new(CustomClaimTypes.TenantId, claims.TenantId)
        };

        // 添加组织 ID
        if (!string.IsNullOrWhiteSpace(claims.OrganizationId))
        {
            tokenClaims.Add(new Claim(CustomClaimTypes.OrganizationId, claims.OrganizationId));
        }

        // 添加角色
        foreach (var role in claims.Roles)
        {
            tokenClaims.Add(new Claim(ClaimTypes.Role, role));
            tokenClaims.Add(new Claim(CustomClaimTypes.Role, role));
        }

        // 添加权限
        foreach (var permission in claims.Permissions)
        {
            tokenClaims.Add(new Claim(CustomClaimTypes.Permission, permission));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: tokenClaims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    public async Task<RefreshToken> GenerateRefreshTokenAsync(
        Guid userId,
        string tenantId,
        string createdByIp,
        string? deviceInfo = null)
    {
        var now = _clock.GetCurrentInstant();
        var expiresAt = now.Plus(Duration.FromDays(_jwtSettings.RefreshTokenExpirationDays));

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = tenantId,
            Token = GenerateRandomToken(),
            ExpiresAt = expiresAt,
            CreatedAt = now,
            CreatedByIp = createdByIp,
            DeviceInfo = deviceInfo
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    /// <summary>
    /// 验证刷新令牌
    /// </summary>
    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null)
        {
            return null;
        }

        // 使用业务方法检查令牌是否可用
        if (!refreshToken.IsActive(_clock))
        {
            return null;
        }

        return refreshToken;
    }

    /// <summary>
    /// 撤销刷新令牌
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string token, string revokedByIp, string? reason = null)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken != null && !refreshToken.IsRevoked())
        {
            refreshToken.Revoke(_clock, revokedByIp, reason);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 生成随机 Token（SHA256 哈希）
    /// </summary>
    private static string GenerateRandomToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        // 计算 SHA256 哈希
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(randomBytes);

        return Convert.ToBase64String(hashBytes);
    }
}
