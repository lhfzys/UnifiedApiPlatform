using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommand: CommandBase
{
    public Guid? UserId { get; set; }
    public string? OldPassword { get; set; }
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
