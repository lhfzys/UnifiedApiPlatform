using MediatR;
using UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

namespace UnifiedApiPlatform.Api.Endpoints.Auth;

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginEndpoint  : CommandEndpointBase<LoginRequest, LoginCommand, LoginResponse>
{
    public LoginEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "用户登录";
            s.Description = "使用邮箱和密码登录系统";
            s.ExampleRequest = new LoginRequest() { Email = "admin@example.com", Password = "Admin@123" };
        });
    }
}
