using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Common.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// 生成访问令牌
    /// </summary>
    string GenerateAccessToken(TokenClaims claims);

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// 从访问令牌中解析声明
    /// </summary>
    TokenClaims? GetClaimsFromToken(string token);

    /// <summary>
    /// 验证访问令牌（不检查过期时间）
    /// </summary>
    bool ValidateToken(string token, out TokenClaims? claims);
}
