namespace UnifiedApiPlatform.Application.Common.Models;

/// <summary>
/// Token 声明数据
/// </summary>
public class TokenClaims
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string? OrganizationId { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
