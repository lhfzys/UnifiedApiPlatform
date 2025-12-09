using MediatR;
using UnifiedApiPlatform.Application.Features.Roles.Commands.RemoveRoleFromUsers;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class RemoveRoleFromUsersEndpoint: CommandEndpointBase<RemoveRoleFromUsersRequest, RemoveRoleFromUsersCommand>
{
    public RemoveRoleFromUsersEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Delete("roles/{roleId}/users");
        Permissions(PermissionCodes.RolesUpdate);
        Summary(s =>
        {
            s.Summary = "移除用户的角色";
            s.Description = "从多个用户移除指定角色";
        });
    }
}
