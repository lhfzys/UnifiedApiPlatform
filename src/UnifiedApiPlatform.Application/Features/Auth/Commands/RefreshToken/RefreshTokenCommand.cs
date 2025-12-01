using FluentResults;
using MediatR;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand: IRequest<Result<TokenResult>>
{
    public string RefreshToken { get; set; } = null!;
    public string? IpAddress { get; set; }
}
