using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Menus.Commands.DeleteMenu;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 删除菜单端点
/// </summary>
public class DeleteMenuEndpoint : CommandEndpointBase<DeleteMenuRequest, DeleteMenuCommand>
{
    public DeleteMenuEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Delete("menus/{menuId}");
        Permissions(PermissionCodes.MenusDelete);
        Summary(s =>
        {
            s.Summary = "删除菜单";
            s.Description = "软删除菜单，存在子菜单时不能删除";
        });
    }
}
