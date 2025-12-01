using FluentResults;
using MediatR;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest<Result>
{
    public string? RefreshToken { get; set; }
    public string? IpAddress { get; set; }
}
