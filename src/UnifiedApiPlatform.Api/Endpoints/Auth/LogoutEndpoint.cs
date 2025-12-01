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

public class LogoutEndpoint: CommandEndpointBase<LogoutRequest, LogoutCommand>
{
    public LogoutEndpoint(IMediator mediator) : base(mediator)
    {
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
}
