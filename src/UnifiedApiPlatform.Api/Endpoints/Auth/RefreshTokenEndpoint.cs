using MediatR;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Features.Auth.Commands.RefreshToken;

namespace UnifiedApiPlatform.Api.Endpoints.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenEndpoint(IMediator mediator)
    : CommandEndpointBase<RefreshTokenRequest, RefreshTokenCommand, TokenResult>(mediator)
{
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
}
