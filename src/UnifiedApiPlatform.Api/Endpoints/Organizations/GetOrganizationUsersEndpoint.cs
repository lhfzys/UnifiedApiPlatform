using MediatR;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationUsers;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

/// <summary>
/// 获取组织用户列表端点
/// </summary>
public class GetOrganizationUsersEndpoint : QueryEndpointBase<GetOrganizationUsersRequest, GetOrganizationUsersQuery, PagedResult<OrganizationUserDto>>
{
    public GetOrganizationUsersEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("organizations/{organizationId}/users");
        Permissions(PermissionCodes.OrganizationsView);
        Summary(s =>
        {
            s.Summary = "获取组织用户列表";
            s.Description = "分页获取指定组织下的所有用户，可选择是否包含子组织";
        });
    }
}
