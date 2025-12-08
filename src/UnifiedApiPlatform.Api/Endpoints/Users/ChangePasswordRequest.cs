namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class ChangePasswordRequest
{
    public Guid? UserId { get; set; }
    public string? OldPassword { get; set; }
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
