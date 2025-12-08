using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand: CommandBase
{
    public Guid UserId { get; set; }
}
