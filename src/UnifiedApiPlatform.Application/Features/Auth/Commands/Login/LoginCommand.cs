using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

/// <summary>
/// 登录命令
/// </summary>
public class LoginCommand : CommandBase<LoginResponse>
{
    public string Account { get; set; } = null!;
    public string Password { get; set; } = null!;
}

/// <summary>
/// 登录响应
/// </summary>
public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public LoginUserInfo User { get; set; } = null!;
}

/// <summary>
/// 登录用户信息
/// </summary>
public class LoginUserInfo
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Avatar { get; set; }
    public string TenantId { get; set; } = null!;
}
