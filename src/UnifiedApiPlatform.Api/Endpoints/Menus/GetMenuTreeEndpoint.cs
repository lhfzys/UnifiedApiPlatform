using MediatR;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenuTree;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

public class GetMenuTreeRequest
{
    public bool IncludeInactive { get; set; }
    public bool IncludeHidden { get; set; }
}

/// <summary>
/// 获取菜单树端点
/// </summary>
public class GetMenuTreeEndpoint : QueryEndpointBase<GetMenuTreeRequest, GetMenuTreeQuery, List<MenuTreeNodeDto>>
{
    public GetMenuTreeEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("menus/tree");
        Permissions(PermissionCodes.MenusView);
        Summary(s =>
        {
            s.Summary = "获取菜单树";
            s.Description = "获取完整的菜单树形结构";
        });
    }
}
