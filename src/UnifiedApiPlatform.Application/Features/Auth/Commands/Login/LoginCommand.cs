using UnifiedApiPlatform.Application.Common.Commands;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

public class LoginCommand : CommandBase<LoginResponse>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public TokenResult Token { get; set; } = null!;
    public UserInfo User { get; set; } = null!;
}

public class UserInfo
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Avatar { get; set; }
}
