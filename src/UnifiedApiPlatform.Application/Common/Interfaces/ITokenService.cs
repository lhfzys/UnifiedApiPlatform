using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Application.Common.Interfaces;

/// <summary>
/// Token 服务接口
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// 生成访问令牌
    /// </summary>
    string GenerateAccessToken(TokenClaims claims);

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    Task<RefreshToken> GenerateRefreshTokenAsync(
        Guid userId,
        string tenantId,
        string createdByIp,
        string? deviceInfo = null);

    /// <summary>
    /// 验证刷新令牌
    /// </summary>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);

    /// <summary>
    /// 撤销刷新令牌
    /// </summary>
    Task RevokeRefreshTokenAsync(string token, string revokedByIp, string? reason = null);
}
