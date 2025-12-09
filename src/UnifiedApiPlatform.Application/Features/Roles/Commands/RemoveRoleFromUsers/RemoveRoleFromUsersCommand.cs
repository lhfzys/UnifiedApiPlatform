using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.RemoveRoleFromUsers;

public class RemoveRoleFromUsersCommand: CommandBase
{
    public Guid RoleId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
}
