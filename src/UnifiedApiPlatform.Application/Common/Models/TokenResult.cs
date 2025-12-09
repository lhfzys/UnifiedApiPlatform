namespace UnifiedApiPlatform.Application.Common.Models;

/// <summary>
/// Token 结果
/// </summary>
public class TokenResult
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
