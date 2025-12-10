using FluentResults;
using MediatR;
using UnifiedApiPlatform.Application.Common.Commands;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// 刷新令牌命令
/// </summary>
public class RefreshTokenCommand: CommandBase<RefreshTokenResponse>
{
    public string RefreshToken { get; set; } = null!;
}

/// <summary>
/// 刷新令牌响应
/// </summary>
public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
