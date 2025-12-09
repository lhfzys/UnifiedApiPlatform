using MediatR;
using UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizations;
using UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationTree;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class GetOrganizationTreeRequest
{
    public bool IncludeInactive { get; set; }
}

public class GetOrganizationTreeEndpoint : QueryEndpointBase<GetOrganizationTreeRequest, GetOrganizationTreeQuery,
    List<OrganizationTreeNodeDto>>
{
    public GetOrganizationTreeEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("organizations/tree");
        Permissions(PermissionCodes.OrganizationsView);
        Summary(s =>
        {
            s.Summary = "获取组织树";
            s.Description = "获取完整的组织树形结构";
        });
    }
}
