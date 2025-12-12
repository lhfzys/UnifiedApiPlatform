using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.Tenants.Queries.GetTenants;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Tenants;

/// <summary>
/// 查询租户列表端点
/// </summary>
public class GetTenantsEndpoint : Endpoint<GetTenantsQuery>
{
    private readonly IMediator _mediator;

    public GetTenantsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("tenants");
        Permissions(PermissionCodes.TenantsView);
        Summary(s =>
        {
            s.Summary = "查询租户列表";
            s.Description = "分页查询租户列表，支持搜索和筛选";
        });
    }

    public override async Task HandleAsync(GetTenantsQuery req, CancellationToken ct)
    {
        req.TraceId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(req, ct);

        await this.SendResultAsync(result, ct);
    }
}
