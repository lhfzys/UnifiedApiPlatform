using FastEndpoints;
using Mapster;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.Menus.Commands.UpdateMenu;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 更新菜单端点
/// </summary>
public class UpdateMenuEndpoint : Endpoint<UpdateMenuRequest>
{
    private readonly IMediator _mediator;

    public UpdateMenuEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("menus/{menuId}");
        Permissions(PermissionCodes.MenusUpdate);
        Summary(s =>
        {
            s.Summary = "更新菜单";
            s.Description = "更新菜单信息，可以修改父菜单";
        });
    }

    public override async Task HandleAsync(UpdateMenuRequest req, CancellationToken ct)
    {
        var command = req.Adapt<UpdateMenuCommand>();

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
