using Microsoft.AspNetCore.Http;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Infrastructure.Identity.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User
        ?.FindFirst(CustomClaimTypes.UserId)?.Value;

    public string? TenantId => _httpContextAccessor.HttpContext?.User
        ?.FindFirst(CustomClaimTypes.TenantId)?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User
        ?.FindFirst(CustomClaimTypes.Email)?.Value;

    public string? UserName => _httpContextAccessor.HttpContext?.User
        ?.FindFirst(CustomClaimTypes.UserName)?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User
        .FindAll(CustomClaimTypes.Role)
        .Select(c => c.Value) ?? Enumerable.Empty<string>();

    public IEnumerable<string> Permissions => _httpContextAccessor.HttpContext?.User
        .FindAll(CustomClaimTypes.Permission)
        .Select(c => c.Value) ?? Enumerable.Empty<string>();
}
