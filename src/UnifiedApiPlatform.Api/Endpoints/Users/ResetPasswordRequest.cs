namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class ResetPasswordRequest
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
