using MediatR;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleUsers;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class GetRoleUsersEndpoint : QueryEndpointBase<GetRoleUsersRequest, GetRoleUsersQuery, PagedResult<RoleUserDto>>
{
    public GetRoleUsersEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("roles/{roleId}/users");
        Permissions(PermissionCodes.RolesView);
        Summary(s =>
        {
            s.Summary = "获取角色用户列表";
            s.Description = "分页获取指定角色下的所有用户";
        });
    }
}
