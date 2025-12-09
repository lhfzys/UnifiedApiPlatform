using MediatR;
using UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationById;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class GetOrganizationByIdEndpoint: QueryEndpointBase<GetOrganizationByIdRequest, GetOrganizationByIdQuery, OrganizationDetailDto>
{
    public GetOrganizationByIdEndpoint(IMediator mediator) : base(mediator)
    {
    }
    public override void Configure()
    {
        Get("organizations/{organizationId}");
        Permissions(PermissionCodes.OrganizationsView);
        Summary(s =>
        {
            s.Summary = "获取组织详情";
            s.Description = "获取组织的详细信息";
        });
    }
}
