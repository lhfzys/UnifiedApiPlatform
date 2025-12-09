using MediatR;
using UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleById;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class GetRoleByIdEndpoint: QueryEndpointBase<GetRoleByIdRequest, GetRoleByIdQuery, RoleDetailDto>
{
    public GetRoleByIdEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("roles/{roleId}");
        Permissions(PermissionCodes.RolesView);
        Summary(s =>
        {
            s.Summary = "获取角色详情";
            s.Description = "获取角色的详细信息，包括权限列表";
        });
    }
}
