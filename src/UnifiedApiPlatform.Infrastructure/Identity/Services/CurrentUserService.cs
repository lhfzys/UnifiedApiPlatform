using System.Security.Claims;
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

    /// <summary>
    /// 用户 ID
    /// </summary>
    public string? UserId => GetClaimValue(CustomClaimTypes.UserId)
                             ?? GetClaimValue(ClaimTypes.NameIdentifier);

    /// <summary>
    /// 租户 ID
    /// </summary>
    public string? TenantId => GetClaimValue(CustomClaimTypes.TenantId);

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email => GetClaimValue(CustomClaimTypes.Email)
                            ?? GetClaimValue(ClaimTypes.Email);

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName => GetClaimValue(CustomClaimTypes.UserName)
                               ?? GetClaimValue(ClaimTypes.Name);

    /// <summary>
    /// 是否已认证
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// 角色列表
    /// </summary>
    public IEnumerable<string> Roles => GetClaimValues(CustomClaimTypes.Role)
        .Concat(GetClaimValues(ClaimTypes.Role))
        .Distinct();

    /// <summary>
    /// 权限列表
    /// </summary>
    public IEnumerable<string> Permissions => GetClaimValues(CustomClaimTypes.Permission);

    /// <summary>
    /// 获取 Claim 值
    /// </summary>
    private string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User
            ?.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// 获取多个 Claim 值
    /// </summary>
    private IEnumerable<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User
                   ?.FindAll(claimType)
                   .Select(c => c.Value)
               ?? Enumerable.Empty<string>();
    }
}
