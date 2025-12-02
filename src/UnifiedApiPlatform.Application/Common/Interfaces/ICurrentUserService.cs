namespace UnifiedApiPlatform.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? TenantId { get; }
    string? Email { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<string> Permissions { get; }
}
