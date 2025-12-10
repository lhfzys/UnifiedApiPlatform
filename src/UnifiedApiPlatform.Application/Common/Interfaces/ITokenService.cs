using NodaTime;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Application.Common.Interfaces;

/// <summary>
/// 令牌服务接口
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// 生成访问令牌和刷新令牌
    /// </summary>
    Task<(string AccessToken, string RefreshToken, Instant ExpiresAt)> GenerateTokensAsync(
        Guid userId,
        string userName,
        string email,
        string tenantId,
        List<string> permissions,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证刷新令牌
    /// </summary>
    Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销刷新令牌
    /// </summary>
    Task RevokeRefreshTokenAsync(
        string token,
        string ipAddress,
        string? reason = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销用户的所有刷新令牌
    /// </summary>
    Task<int> RevokeAllUserTokensAsync(
        Guid userId,
        string ipAddress,
        string? reason = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 刷新令牌验证结果
/// </summary>
public class RefreshTokenValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public RefreshToken? RefreshToken { get; set; }
    public User? User { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }

    public static RefreshTokenValidationResult Success(RefreshToken refreshToken, User user)
    {
        return new RefreshTokenValidationResult
        {
            IsValid = true,
            RefreshToken = refreshToken,
            User = user,
            UserId = user.Id,
            UserName = user.UserName
        };
    }

    public static RefreshTokenValidationResult Failure(string errorMessage, Guid? userId = null, string? userName = null)
    {
        return new RefreshTokenValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage,
            UserId = userId,
            UserName = userName
        };
    }
}
