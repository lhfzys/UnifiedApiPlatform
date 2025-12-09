using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetRoleMenus;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 获取角色菜单端点
/// </summary>
public class GetRoleMenusEndpoint : QueryEndpointBase<GetRoleMenusRequest, GetRoleMenusQuery, List<MenuTreeNodeDto>>
{
    public GetRoleMenusEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("roles/{roleId}/menus");
        Permissions(PermissionCodes.RolesView);
        Summary(s =>
        {
            s.Summary = "获取角色菜单";
            s.Description = "获取指定角色的菜单树";
        });
    }
}
