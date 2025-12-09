using MediatR;
using UnifiedApiPlatform.Application.Features.Organizations.Commands.CreateOrganization;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Organizations;

public class CreateOrganizationEndpoint: CommandEndpointBase<CreateOrganizationRequest, CreateOrganizationCommand, CreateOrganizationResponse>
{
    public CreateOrganizationEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("organizations");
        Permissions(PermissionCodes.OrganizationsCreate);
        Summary(s =>
        {
            s.Summary = "创建组织";
            s.Description = "创建新组织，可以指定父组织";
        });
    }
}
