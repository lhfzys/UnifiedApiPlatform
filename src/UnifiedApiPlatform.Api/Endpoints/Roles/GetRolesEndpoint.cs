using MediatR;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoles;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class GetRolesEndpoint: QueryEndpointBase<GetRolesRequest, GetRolesQuery, PagedResult<RoleDto>>
{
    public GetRolesEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("roles");
        Permissions(PermissionCodes.RolesView);
        Summary(s =>
        {
            s.Summary = "获取角色列表";
            s.Description = "分页获取角色列表，支持搜索和筛选";
        });
    }
}
