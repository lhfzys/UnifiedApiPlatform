using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommand: CommandBase
{
    public Guid OrganizationId { get; set; }
}
