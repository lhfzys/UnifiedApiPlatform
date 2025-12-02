using FastEndpoints;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.PreProcessors;

/// <summary>
/// 权限验证预处理器
/// 基于端点配置的权限要求进行验证
/// </summary>
public class PermissionAuthorizationPreProcessor<TRequest> : IPreProcessor<TRequest>
{
    private readonly ILogger<PermissionAuthorizationPreProcessor<TRequest>> _logger;

    public PermissionAuthorizationPreProcessor(ILogger<PermissionAuthorizationPreProcessor<TRequest>> logger)
    {
        _logger = logger;
    }

    public async Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
    {
        var endpoint = ctx.HttpContext.GetEndpoint();
        if (endpoint == null)
        {
            return;
        }

        // 获取端点的权限元数据
        var permissionMetadata = endpoint.Metadata.GetMetadata<PermissionMetadata>();
        if (permissionMetadata == null)
        {
            // 没有权限要求，跳过
            return;
        }

        // 检查用户是否已认证
        if (!ctx.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            _logger.LogWarning("未认证的用户尝试访问需要权限的端点: {Endpoint}", endpoint.DisplayName);

            await ctx.HttpContext.SendErrorResponseAsync(
                ErrorCodes.Unauthorized,
                statusCode: 401,
                ct: ct
            );
            return;
        }

        // 获取用户的权限 Claims
        var userPermissions = ctx.HttpContext.User.Claims
            .Where(c => c.Type == CustomClaimTypes.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _logger.LogDebug(
            "用户权限: {UserPermissions}, 需要的权限: {RequiredPermissions}, 验证模式: {Mode}",
            string.Join(", ", userPermissions),
            string.Join(", ", permissionMetadata.Permissions),
            permissionMetadata.RequireAll ? "AND" : "OR"
        );

        // 验证权限
        bool hasPermission;
        if (permissionMetadata.RequireAll)
        {
            // AND 模式：必须拥有所有权限
            hasPermission = permissionMetadata.Permissions.All(p => userPermissions.Contains(p));
        }
        else
        {
            // OR 模式：至少拥有一个权限
            hasPermission = permissionMetadata.Permissions.Any(p => userPermissions.Contains(p));
        }

        if (!hasPermission)
        {
            _logger.LogWarning(
                "用户 {UserId} 权限不足，尝试访问: {Endpoint}, 需要权限: {RequiredPermissions}",
                ctx.HttpContext.User.FindFirst(CustomClaimTypes.UserId)?.Value,
                endpoint.DisplayName,
                string.Join(", ", permissionMetadata.Permissions)
            );

            await ctx.HttpContext.SendErrorResponseAsync(
                ErrorCodes.PermissionDenied,
                statusCode: 403,
                ct: ct
            );
        }
    }
}

/// <summary>
/// 权限元数据
/// </summary>
public class PermissionMetadata
{
    public string[] Permissions { get; set; } = Array.Empty<string>();
    public bool RequireAll { get; set; } = false;
}
