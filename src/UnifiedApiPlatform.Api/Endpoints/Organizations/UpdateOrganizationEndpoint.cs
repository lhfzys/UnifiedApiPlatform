using FastEndpoints;
using Mapster;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Organizations.Commands.UpdateOrganization;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

/// <summary>
/// 更新组织端点
/// </summary>
public class UpdateOrganizationEndpoint : Endpoint<UpdateOrganizationRequest>
{
    private readonly IMediator _mediator;

    public UpdateOrganizationEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("organizations/{organizationId}");

        Permissions(PermissionCodes.OrganizationsUpdate);
        Summary(s =>
        {
            s.Summary = "更新组织";
            s.Description = "更新组织信息，可以修改父组织";
        });
    }

    public override async Task HandleAsync(UpdateOrganizationRequest req, CancellationToken ct)
    {
        var command = req.Adapt<UpdateOrganizationCommand>();

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
