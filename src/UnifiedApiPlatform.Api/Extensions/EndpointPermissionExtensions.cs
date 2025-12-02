using FastEndpoints;
using UnifiedApiPlatform.Api.PreProcessors;

namespace UnifiedApiPlatform.Api.Extensions;

/// <summary>
/// 端点权限配置扩展方法
/// </summary>
public static class EndpointPermissionExtensions
{
    /// <summary>
    /// 要求单个权限
    /// </summary>
    public static void RequirePermission(this IEndpointConventionBuilder builder, string permission)
    {
        builder.WithMetadata(new PermissionMetadata { Permissions = new[] { permission }, RequireAll = false });
    }

    /// <summary>
    /// 要求多个权限（OR 模式：至少拥有一个）
    /// </summary>
    public static void RequirePermissions(this IEndpointConventionBuilder builder, params string[] permissions)
    {
        builder.WithMetadata(new PermissionMetadata { Permissions = permissions, RequireAll = false });
    }

    /// <summary>
    /// 要求所有权限（AND 模式：必须拥有所有）
    /// </summary>
    public static void RequireAllPermissions(this IEndpointConventionBuilder builder, params string[] permissions)
    {
        builder.WithMetadata(new PermissionMetadata { Permissions = permissions, RequireAll = true });
    }
}

/// <summary>
/// FastEndpoints 配置扩展（用于 Configure() 方法）
/// </summary>
public static class FastEndpointPermissionExtensions
{
    /// <summary>
    /// 要求单个权限
    /// </summary>
    public static void Permissions(this EndpointDefinition ep, string permission)
    {
        ep.Description(x => x.WithMetadata(new PermissionMetadata
        {
            Permissions = new[] { permission },
            RequireAll = false
        }));
    }

    /// <summary>
    /// 要求多个权限（OR 模式）
    /// </summary>
    public static void Permissions(this EndpointDefinition ep, params string[] permissions)
    {
        ep.Description(x => x.WithMetadata(new PermissionMetadata
        {
            Permissions = permissions,
            RequireAll = false
        }));
    }

    /// <summary>
    /// 要求所有权限（AND 模式）
    /// </summary>
    public static void RequireAllPermissions(this EndpointDefinition ep, params string[] permissions)
    {
        ep.Description(x => x.WithMetadata(new PermissionMetadata
        {
            Permissions = permissions,
            RequireAll = true
        }));
    }
}
