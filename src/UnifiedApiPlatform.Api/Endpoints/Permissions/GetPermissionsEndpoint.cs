using MediatR;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Permissions.Queries.GetPermissions;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Permissions;

/// <summary>
/// 获取权限列表端点
/// </summary>
public class
    GetPermissionsEndpoint : QueryEndpointBase<GetPermissionsRequest, GetPermissionsQuery, List<PermissionCategoryDto>>
{
    public GetPermissionsEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Get("permissions");
        Permissions(PermissionCodes.PermissionsView);
        Summary(s =>
        {
            s.Summary = "获取权限列表";
            s.Description = "获取系统所有权限，按分类分组";
        });
    }
}
