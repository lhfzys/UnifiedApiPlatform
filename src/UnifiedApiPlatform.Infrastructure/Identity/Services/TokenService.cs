using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Infrastructure.Options;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Infrastructure.Identity.Services;

/// <summary>
/// 令牌服务实现
/// </summary>
public class TokenService : ITokenService
{
    private readonly IApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly IClock _clock;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IApplicationDbContext context,
        IOptions<JwtSettings> jwtSettings,
        IClock clock,
        ILogger<TokenService> logger)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _clock = clock;
        _logger = logger;
    }

    public async Task<(string AccessToken, string RefreshToken, Instant ExpiresAt)> GenerateTokensAsync(
        Guid userId,
        string userName,
        string email,
        string tenantId,
        List<string> permissions,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 生成 Access Token
            var accessToken = GenerateAccessToken(userId, userName, email, tenantId, permissions);

            // 生成 Refresh Token
            var (refreshToken, expiresAt) = await GenerateRefreshTokenAsync(
                userId,
                tenantId,
                cancellationToken);

            return (accessToken, refreshToken, expiresAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成令牌失败: UserId: {UserId}", userId);
            throw;
        }
    }

    public async Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 计算 Token 的 SHA256 哈希
            var tokenHash = ComputeSha256Hash(token);

            // 查询 Refresh Token（包含用户信息和权限）
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(rt => rt.Token == tokenHash, cancellationToken);

            // Token 不存在
            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh Token 不存在");
                return RefreshTokenValidationResult.Failure("Refresh Token 不存在");
            }

            var user = refreshToken.User;

            // Token 已撤销
            if (refreshToken.IsRevoked())
            {
                _logger.LogWarning("Refresh Token 已撤销: UserId: {UserId}", user.Id);
                return RefreshTokenValidationResult.Failure(
                    "Refresh Token 已撤销",
                    user.Id,
                    user.UserName);
            }

            // Token 已过期
            if (refreshToken.IsExpired(_clock))
            {
                _logger.LogWarning("Refresh Token 已过期: UserId: {UserId}", user.Id);
                return RefreshTokenValidationResult.Failure(
                    "Refresh Token 已过期",
                    user.Id,
                    user.UserName);
            }

            // 验证通过
            return RefreshTokenValidationResult.Success(refreshToken, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证 Refresh Token 失败");
            return RefreshTokenValidationResult.Failure("验证 Refresh Token 失败");
        }
    }

    public async Task RevokeRefreshTokenAsync(
        string token,
        string ipAddress,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 计算 Token 的 SHA256 哈希
            var tokenHash = ComputeSha256Hash(token);

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == tokenHash, cancellationToken);

            if (refreshToken == null)
            {
                _logger.LogWarning("尝试撤销不存在的 Refresh Token");
                return;
            }

            // 已经撤销，跳过
            if (refreshToken.IsRevoked())
            {
                return;
            }

            // 使用实体方法撤销
            refreshToken.Revoke(_clock, ipAddress, reason);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Refresh Token 已撤销: UserId: {UserId}, Reason: {Reason}",
                refreshToken.UserId,
                reason ?? "未指定");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "撤销 Refresh Token 失败");
            throw;
        }
    }

    public async Task<int> RevokeAllUserTokensAsync(
        Guid userId,
        string ipAddress,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 查询用户所有未撤销的 Refresh Token
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            if (refreshTokens.Count == 0)
            {
                return 0;
            }

            // 使用实体方法撤销所有 Token
            foreach (var token in refreshTokens)
            {
                token.Revoke(_clock, ipAddress, reason);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "撤销用户所有 Refresh Token: UserId: {UserId}, Count: {Count}, Reason: {Reason}",
                userId,
                refreshTokens.Count,
                reason ?? "未指定");

            return refreshTokens.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "撤销用户所有 Refresh Token 失败: UserId: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// 生成 Access Token (JWT)
    /// </summary>
    private string GenerateAccessToken(
        Guid userId,
        string userName,
        string email,
        string tenantId,
        List<string> permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(CustomClaimTypes.UserId, userId.ToString()),
            new(CustomClaimTypes.UserName, userName),
            new(CustomClaimTypes.Email, email),
            new(CustomClaimTypes.TenantId, tenantId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, email)
        };

        // 添加权限声明
        claims.AddRange(permissions.Select(p => new Claim(CustomClaimTypes.Permission, p)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// 生成 Refresh Token
    /// </summary>
    private async Task<(string Token, Instant ExpiresAt)> GenerateRefreshTokenAsync(
        Guid userId,
        string tenantId,
        CancellationToken cancellationToken)
    {
        // 生成随机 Token（64 字节）
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        // 计算 SHA256 哈希（存储到数据库）
        var tokenHash = ComputeSha256Hash(token);

        // 计算过期时间
        var expiresAt = _clock.GetCurrentInstant()
            .Plus(Duration.FromDays(_jwtSettings.RefreshTokenExpirationDays));

        // 创建 RefreshToken 实体
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = tenantId,
            Token = tokenHash, // 存储哈希值
            ExpiresAt = expiresAt,
            CreatedAt = _clock.GetCurrentInstant(),
            CreatedByIp = "Unknown", // 会在 Handler 中更新
            DeviceInfo = null
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 返回原始 Token（不是哈希）
        return (token, expiresAt);
    }

    /// <summary>
    /// 计算 SHA256 哈希
    /// </summary>
    private static string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
