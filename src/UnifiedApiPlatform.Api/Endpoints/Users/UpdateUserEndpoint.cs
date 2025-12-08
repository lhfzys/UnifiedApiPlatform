using FastEndpoints;
using Mapster;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.Users.Commands.UpdateUser;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class UpdateUserEndpoint : Endpoint<UpdateUserRequest>
{
    private readonly IMediator _mediator;

    public UpdateUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("users/{id}");
        Permissions(PermissionCodes.UsersUpdate);
        Summary(s =>
        {
            s.Summary = "更新用户";
            s.Description = "更新用户信息，支持部分更新和乐观并发控制";
        });
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var command = req.Adapt<UpdateUserCommand>();
        if (!string.IsNullOrEmpty(req.RowVersion))
        {
            try
            {
                command.RowVersion = Convert.FromBase64String(req.RowVersion);
            }
            catch
            {
                await this.SendBusinessErrorAsync("无效的行版本", ct: ct);
                return;
            }
        }

        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        command.UserAgent = HttpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown";
        command.TraceId = HttpContext.TraceIdentifier;
        var result = await _mediator.Send(command, ct);
        await this.SendResultAsync(result, ct);
    }
}
