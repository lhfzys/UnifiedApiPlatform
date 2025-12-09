using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Roles.Commands.AssignPermissions;

public class AssignPermissionsCommand: CommandBase
{
    public Guid RoleId { get; set; }
    public List<string> PermissionCodes { get; set; } = new();
}
