using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : CommandBase
{
    public Guid RoleId { get; set; }
}
