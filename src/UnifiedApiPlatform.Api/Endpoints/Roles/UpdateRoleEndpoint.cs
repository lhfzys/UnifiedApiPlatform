using FastEndpoints;
using Mapster;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Roles.Commands.UpdateRole;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class UpdateRoleEndpoint : Endpoint<UpdateRoleRequest>
{
    private readonly IMediator _mediator;

    public UpdateRoleEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("roles/{roleId}");
        Permissions(PermissionCodes.RolesUpdate);
        Summary(s =>
        {
            s.Summary = "更新角色";
            s.Description = "更新角色基本信息（不包含权限，权限需单独管理）";
        });
    }

    public override async Task HandleAsync(UpdateRoleRequest req, CancellationToken ct)
    {
        var command = req.Adapt<UpdateRoleCommand>();

        // 转换 Base64 RowVersion
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

        // 注入 HTTP 上下文
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        command.UserAgent = HttpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown";
        command.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(command, ct);

        await this.SendResultAsync(result, ct);
    }
}
