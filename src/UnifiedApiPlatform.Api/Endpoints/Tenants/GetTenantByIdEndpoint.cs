using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.Tenants.Queries.GetTenantById;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Tenants;

/// <summary>
/// 根据 ID 查询租户端点
/// </summary>
public class GetTenantByIdEndpoint : Endpoint<GetTenantByIdQuery>
{
    private readonly IMediator _mediator;

    public GetTenantByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("tenants/{Id}");
        Permissions(PermissionCodes.TenantsView);
        Summary(s =>
        {
            s.Summary = "查询租户详情";
            s.Description = "根据租户 ID 查询详细信息，包括统计数据";
        });
    }

    public override async Task HandleAsync(GetTenantByIdQuery req, CancellationToken ct)
    {
        req.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(req, ct);

        await this.SendResultAsync(result, ct);
    }
}
