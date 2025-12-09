using MediatR;
using UnifiedApiPlatform.Application.Features.Roles.Commands.DeleteRole;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class DeleteRoleEndpoint: CommandEndpointBase<DeleteRoleRequest, DeleteRoleCommand>
{
    public DeleteRoleEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Delete("roles/{roleId}");
        Permissions(PermissionCodes.RolesDelete);
        Summary(s =>
        {
            s.Summary = "删除角色";
            s.Description = "软删除角色，角色正在使用中时不能删除";
        });
    }
}
