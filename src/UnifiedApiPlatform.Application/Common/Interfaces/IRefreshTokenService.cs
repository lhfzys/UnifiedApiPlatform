using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    /// <summary>
    /// 创建刷新令牌
    /// </summary>
    Task<RefreshToken> CreateRefreshTokenAsync(
        Guid userId,
        string tenantId,
        string token,
        string ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证刷新令牌
    /// </summary>
    Task<RefreshToken?> ValidateRefreshTokenAsync(
        string token,
        string ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销刷新令牌
    /// </summary>
    Task RevokeRefreshTokenAsync(
        string token,
        string ipAddress,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销用户的所有刷新令牌
    /// </summary>
    Task RevokeAllUserTokensAsync(
        Guid userId,
        string ipAddress,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理过期的刷新令牌
    /// </summary>
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}
