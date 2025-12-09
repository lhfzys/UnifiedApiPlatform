using MediatR;
using UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizations;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class GetOrganizationsEndpoint : QueryEndpointBase<GetOrganizationsRequest, GetOrganizationsQuery, List<OrganizationDto>>
{
    public GetOrganizationsEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("organizations");
        Permissions(PermissionCodes.OrganizationsView);
        Summary(s =>
        {
            s.Summary = "获取组织列表";
            s.Description = "获取组织列表，支持搜索和筛选";
        });
    }
}
