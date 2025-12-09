using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Organizations.Commands.AssignUsersToOrganization;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

/// <summary>
/// 批量分配用户到组织端点
/// </summary>
public class AssignUsersToOrganizationEndpoint : CommandEndpointBase<AssignUsersToOrganizationRequest, AssignUsersToOrganizationCommand>
{
    public AssignUsersToOrganizationEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("organizations/{organizationId}/users");
        Permissions(PermissionCodes.OrganizationsUpdate);
        Summary(s =>
        {
            s.Summary = "批量分配用户到组织";
            s.Description = "将多个用户分配到指定组织";
        });
    }
}
