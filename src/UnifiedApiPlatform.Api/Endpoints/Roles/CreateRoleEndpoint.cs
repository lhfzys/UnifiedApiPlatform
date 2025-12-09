using MediatR;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Roles.Commands.CreateRole;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class CreateRoleEndpoint: CommandEndpointBase<CreateRoleRequest, CreateRoleCommand, CreateRoleResponse>
{
    public CreateRoleEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("roles");
        Permissions(PermissionCodes.RolesCreate);
        Summary(s =>
        {
            s.Summary = "创建角色";
            s.Description = "创建新角色，需要 roles.create 权限";
        });
    }
}
