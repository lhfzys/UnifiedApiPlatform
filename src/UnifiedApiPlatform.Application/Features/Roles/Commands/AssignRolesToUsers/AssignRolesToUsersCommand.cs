using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.AssignRolesToUsers;

public class AssignRolesToUsersCommand: CommandBase
{
    public Guid RoleId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
}
