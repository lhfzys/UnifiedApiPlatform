using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.Auth.Commands.Logout;

namespace UnifiedApiPlatform.Api.Endpoints.Auth;

public class LogoutRequest
{
    public string? RefreshToken { get; set; }
}

public class LogoutEndpoint : Endpoint<LogoutRequest>
{
    private readonly IMediator _mediator;

    public LogoutEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("auth/logout");
        Summary(s =>
        {
            s.Summary = "用户登出";
            s.Description = "登出并撤销刷新令牌";
        });
    }

    public override async Task HandleAsync(LogoutRequest req, CancellationToken ct)
    {
        var command = new LogoutCommand
        {
            RefreshToken = req.RefreshToken, IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await this.SendOkAsync("登出成功", ct);
        }
        else
        {
            await this.SendErrorAsync(result.Errors.First().Message, ct: ct);
        }
    }
}
