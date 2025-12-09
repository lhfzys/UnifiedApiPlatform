using MediatR;
using UnifiedApiPlatform.Application.Features.Roles.Commands.AssignPermissions;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class AssignPermissionsEndpoint : CommandEndpointBase<AssignPermissionsRequest, AssignPermissionsCommand>
{
    public AssignPermissionsEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Put("roles/{roleId}/permissions");
        Permissions(PermissionCodes.RolesUpdate);
        Summary(s =>
        {
            s.Summary = "分配权限给角色";
            s.Description = "覆盖角色的所有权限（传入空数组将移除所有权限）";
        });
    }
}
