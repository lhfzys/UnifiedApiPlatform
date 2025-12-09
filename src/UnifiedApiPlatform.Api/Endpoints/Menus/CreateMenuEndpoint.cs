using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Menus.Commands.CreateMenu;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 创建菜单端点
/// </summary>
public class CreateMenuEndpoint : CommandEndpointBase<CreateMenuRequest, CreateMenuCommand, CreateMenuResponse>
{
    public CreateMenuEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("menus");
        Permissions(PermissionCodes.MenusCreate);
        Summary(s =>
        {
            s.Summary = "创建菜单";
            s.Description = "创建新菜单，可以指定父菜单";
        });
    }
}
