using FluentResults;
using MediatR;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
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
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
