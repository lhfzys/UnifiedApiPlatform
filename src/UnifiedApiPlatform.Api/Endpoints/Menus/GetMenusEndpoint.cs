using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 获取菜单列表端点
/// </summary>
public class GetMenusEndpoint : QueryEndpointBase<GetMenusRequest, GetMenusQuery, List<MenuDto>>
{
    public GetMenusEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("menus");
        Permissions(PermissionCodes.MenusView);
        Summary(s =>
        {
            s.Summary = "获取菜单列表";
            s.Description = "获取菜单列表，支持搜索和筛选";
        });
    }
}
