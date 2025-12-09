using MediatR;
using UnifiedApiPlatform.Application.Features.Roles.Commands.AssignRolesToUsers;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class AssignRolesToUsersEndpoint: CommandEndpointBase<AssignRolesToUsersRequest, AssignRolesToUsersCommand>
{
    public AssignRolesToUsersEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("roles/{roleId}/users");
        Permissions(PermissionCodes.RolesUpdate);
        Summary(s =>
        {
            s.Summary = "批量分配角色给用户";
            s.Description = "将指定角色分配给多个用户";
        });
    }
}
