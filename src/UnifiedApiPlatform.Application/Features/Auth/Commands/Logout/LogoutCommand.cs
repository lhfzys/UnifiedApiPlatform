using UnifiedApiPlatform.Application.Common.Commands;


namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Logout;

public class LogoutCommand  : CommandBase
{
    public string? RefreshToken { get; set; }
}
