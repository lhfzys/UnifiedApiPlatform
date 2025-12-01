using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

namespace UnifiedApiPlatform.Api.Endpoints.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest>
{
    private readonly IMediator _mediator;

    public RefreshTokenEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("auth/refresh");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "刷新访问令牌";
            s.Description = "使用刷新令牌获取新的访问令牌";
        });
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = req.RefreshToken, IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        var result = await _mediator.Send(command, ct);

        await this.SendResultAsync(result, ct);
    }
}
