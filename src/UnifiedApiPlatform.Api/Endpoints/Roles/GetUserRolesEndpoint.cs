using MediatR;
using UnifiedApiPlatform.Application.Features.Roles.Queries.GetUserRoles;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class GetUserRolesEndpoint: QueryEndpointBase<GetUserRolesRequest, GetUserRolesQuery, List<UserRoleDto>>
{
    public GetUserRolesEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("users/{userId}/roles");
        Permissions(PermissionCodes.UsersView);
        Summary(s =>
        {
            s.Summary = "获取用户角色列表";
            s.Description = "获取指定用户所有角色";
        });
    }
}
