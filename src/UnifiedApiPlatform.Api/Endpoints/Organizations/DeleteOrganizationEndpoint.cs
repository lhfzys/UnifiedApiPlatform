using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Endpoints;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Features.Organizations.Commands.DeleteOrganization;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

/// <summary>
/// 删除组织端点
/// </summary>
public class DeleteOrganizationEndpoint : CommandEndpointBase<DeleteOrganizationRequest, DeleteOrganizationCommand>
{
    public DeleteOrganizationEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Delete("organizations/{organizationId}");
        Permissions(PermissionCodes.OrganizationsDelete);
        Summary(s =>
        {
            s.Summary = "删除组织";
            s.Description = "软删除组织，存在子组织或用户时不能删除";
        });
    }
}
