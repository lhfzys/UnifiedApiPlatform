using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenuById;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 获取菜单详情端点
/// </summary>
public class GetMenuByIdEndpoint : QueryEndpointBase<GetMenuByIdRequest, GetMenuByIdQuery, MenuDetailDto>
{
    public GetMenuByIdEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("menus/{menuId}");
        Permissions(PermissionCodes.MenusView);
        Summary(s =>
        {
            s.Summary = "获取菜单详情";
            s.Description = "获取菜单的详细信息";
        });
    }
}
