using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetCurrentUserMenus;
using UnifiedApiPlatform.Application.Features.Menus.Queries.GetMenus;

namespace UnifiedApiPlatform.Api.Endpoints.Menus;

/// <summary>
/// 获取当前用户菜单端点
/// </summary>
public class GetCurrentUserMenusEndpoint : EndpointWithoutRequest<List<MenuTreeNodeDto>>
{
    private readonly IMediator _mediator;

    public GetCurrentUserMenusEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("menus/current-user");

        // 已认证用户即可访问，不需要特定权限

        Summary(s =>
        {
            s.Summary = "获取当前用户菜单";
            s.Description = "获取当前登录用户的菜单树（用于前端渲染导航）";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetCurrentUserMenusQuery();

        var result = await _mediator.Send(query, ct);

        if (result.IsSuccess)
        {
            await Send.OkAsync(result.Value, ct);
        }
        else
        {
            await Send.ResultAsync(Results.BadRequest(new
            {
                success = false, message = result.Errors.FirstOrDefault()?.Message ?? "获取菜单失败"
            }));
        }
    }
}
