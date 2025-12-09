using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Menus.Commands.AssignMenusToRole;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 分配菜单给角色端点
/// </summary>
public class AssignMenusToRoleEndpoint : CommandEndpointBase<AssignMenusToRoleRequest, AssignMenusToRoleCommand>
{
    public AssignMenusToRoleEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Put("roles/{roleId}/menus");
        Permissions(PermissionCodes.RolesUpdate);
        Summary(s =>
        {
            s.Summary = "分配菜单给角色";
            s.Description = "覆盖角色的所有菜单（传入空数组将移除所有菜单）";
        });
    }
}
