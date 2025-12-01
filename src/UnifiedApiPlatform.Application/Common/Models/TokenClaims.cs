namespace UnifiedApiPlatform.Application.Common.Models;

public class TokenClaims
{
    public string UserId { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public string? OrganizationId { get; set; }
}
