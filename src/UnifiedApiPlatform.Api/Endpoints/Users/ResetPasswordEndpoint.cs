using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Users.Commands.ChangePassword;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

/// <summary>
/// 重置用户密码端点（管理员功能）
/// </summary>
public class ResetPasswordEndpoint : Endpoint<ResetPasswordRequest>
{
    private readonly IMediator _mediator;

    public ResetPasswordEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("users/{userId}/reset-password");
        Permissions(PermissionCodes.UsersUpdate);
        Summary(s =>
        {
            s.Summary = "重制用户密码";
            s.Description = "管理员重置用户密码，不需要原密码";
        });
    }

    public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        // 映射到 ChangePasswordCommand
        var command = new ChangePasswordCommand
        {
            UserId = req.UserId,
            OldPassword = null,
            NewPassword = req.NewPassword,
            ConfirmPassword = req.ConfirmPassword,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown",
            TraceId = HttpContext.TraceIdentifier
        };

        var result = await _mediator.Send(command, ct);

        await this.SendResultAsync(result, ct);
    }
}
