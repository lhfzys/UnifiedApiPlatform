using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.Auth.Commands.Login;
using UnifiedApiPlatform.Shared.Helpers;

namespace UnifiedApiPlatform.Api.Endpoints.Auth;

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginEndpoint : Endpoint<LoginRequest>
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoginEndpoint> _logger;

    public LoginEndpoint(IMediator mediator, ILogger<LoginEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
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

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // 调试日志
        var acceptLanguage = HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
        var culture = CultureHelper.GetCurrentCulture(HttpContext);
        _logger.LogInformation("Accept-Language: {AcceptLanguage}, Parsed Culture: {Culture}",
            acceptLanguage, culture);

        var command = new LoginCommand
        {
            Email = req.Email,
            Password = req.Password,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
        };

        var result = await _mediator.Send(command, ct);

        await this.SendResultAsync(result, ct);
    }
}
