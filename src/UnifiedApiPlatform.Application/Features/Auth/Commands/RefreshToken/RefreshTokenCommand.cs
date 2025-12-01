using FluentResults;
using MediatR;
using UnifiedApiPlatform.Application.Common.Commands;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand: CommandBase<TokenResult>
{
    public string RefreshToken { get; set; } = null!;
}
