using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Infrastructure.Options;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Infrastructure.Identity.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;

        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public string GenerateAccessToken(TokenClaims claims)
    {
        var tokenClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, claims.UserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, claims.Email),
            new(CustomClaimTypes.UserId, claims.UserId),
            new(CustomClaimTypes.TenantId, claims.TenantId),
            new(CustomClaimTypes.UserName, claims.UserName),
            new(CustomClaimTypes.Email, claims.Email)
        };

        // 添加组织ID
        if (!string.IsNullOrWhiteSpace(claims.OrganizationId))
        {
            tokenClaims.Add(new Claim(CustomClaimTypes.OrganizationId, claims.OrganizationId));
        }

        // 添加角色
        tokenClaims.AddRange(claims.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        tokenClaims.AddRange(claims.Roles.Select(role => new Claim(CustomClaimTypes.Roles, role)));

        // 添加权限
        tokenClaims.AddRange(claims.Permissions.Select(permission =>
            new Claim(CustomClaimTypes.Permissions, permission)));

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

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public TokenClaims? GetClaimsFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);

            return ExtractClaims(principal);
        }
        catch
        {
            return null;
        }
    }

    public bool ValidateToken(string token, out TokenClaims? claims)
    {
        claims = null;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = _tokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false; // 不验证过期时间

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            claims = ExtractClaims(principal);

            return claims != null;
        }
        catch
        {
            return false;
        }
    }

    private static TokenClaims? ExtractClaims(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirst(CustomClaimTypes.UserId)?.Value;
        var tenantId = principal.FindFirst(CustomClaimTypes.TenantId)?.Value;
        var email = principal.FindFirst(CustomClaimTypes.Email)?.Value;
        var userName = principal.FindFirst(CustomClaimTypes.UserName)?.Value;

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(tenantId) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(userName))
        {
            return null;
        }

        return new TokenClaims
        {
            UserId = userId,
            TenantId = tenantId,
            Email = email,
            UserName = userName,
            OrganizationId = principal.FindFirst(CustomClaimTypes.OrganizationId)?.Value,
            Roles = principal.FindAll(CustomClaimTypes.Roles).Select(c => c.Value).ToList(),
            Permissions = principal.FindAll(CustomClaimTypes.Permissions).Select(c => c.Value).ToList()
        };
    }
}
