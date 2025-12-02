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
        var logger = ctx.HttpContext.RequestServices
            .GetRequiredService<ILogger<PermissionAuthorizationPreProcessor<TRequest>>>();
        logger.LogInformation("========== PermissionAuthorizationPreProcessor 被调用 ==========");
        logger.LogInformation("请求类型: {RequestType}", typeof(TRequest).Name);
        logger.LogInformation("请求路径: {Path}", ctx.HttpContext.Request.Path);
        var endpoint = ctx.HttpContext.GetEndpoint();
        if (endpoint == null)
        {
            logger.LogWarning("无法获取端点信息");
            return;
        }

        logger.LogInformation("端点名称: {DisplayName}", endpoint.DisplayName);
        // 获取端点的权限元数据
        var permissionMetadata = endpoint.Metadata.GetMetadata<PermissionMetadata>();
        if (permissionMetadata == null)
        {
            logger.LogInformation("端点没有权限要求，跳过验证");
            return;
        }

        logger.LogInformation("========== 权限验证开始 ==========");
        logger.LogInformation("需要的权限: {RequiredPermissions}", string.Join(", ", permissionMetadata.Permissions));
        logger.LogInformation("验证模式: {Mode}", permissionMetadata.RequireAll ? "AND (需要全部)" : "OR (至少一个)");

        // 检查用户是否已认证
        var isAuthenticated = ctx.HttpContext.User.Identity?.IsAuthenticated ?? false;
        logger.LogInformation("用户是否已认证: {IsAuthenticated}", isAuthenticated);
        if (!isAuthenticated)
        {
            logger.LogWarning("❌ 未认证的用户尝试访问需要权限的端点");

            await ctx.HttpContext.SendErrorResponseAsync(
                ErrorCodes.Unauthorized,
                statusCode: 401,
                ct: ct
            );
            return;
        }
        // 打印所有 Claims
        logger.LogInformation("用户的所有 Claims:");
        foreach (var claim in ctx.HttpContext.User.Claims)
        {
            logger.LogInformation("  - Type: {Type}, Value: {Value}", claim.Type, claim.Value);
        }

        // 获取用户的权限 Claims
        var userPermissions = ctx.HttpContext.User.Claims
            .Where(c => c.Type == CustomClaimTypes.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        logger.LogInformation("提取的权限列表 ({Count} 个): {Permissions}",
            userPermissions.Count,
            string.Join(", ", userPermissions));

        // 验证权限
        bool hasPermission;
        if (permissionMetadata.RequireAll)
        {
            // AND 模式：必须拥有所有权限
            hasPermission = permissionMetadata.Permissions.All(p => userPermissions.Contains(p));
            _logger.LogInformation("AND 模式验证结果: {Result}", hasPermission);
        }
        else
        {
            // OR 模式：至少拥有一个权限
            hasPermission = permissionMetadata.Permissions.Any(p => userPermissions.Contains(p));
            _logger.LogInformation("OR 模式验证结果: {Result}", hasPermission);

            // 详细显示哪些权限匹配
            foreach (var required in permissionMetadata.Permissions)
            {
                var has = userPermissions.Contains(required);
                _logger.LogInformation("  - 权限 '{Permission}': {HasIt}", required, has ? "✓ 拥有" : "✗ 缺少");
            }
        }

        if (!hasPermission)
        {
            logger.LogWarning("❌ 用户权限不足");
            logger.LogInformation("========== 权限验证结束（失败）==========");

            await ctx.HttpContext.SendErrorResponseAsync(
                ErrorCodes.PermissionDenied,
                statusCode: 403,
                ct: ct
            );
        }
        else
        {
            _logger.LogInformation("✅ 权限验证通过");
            _logger.LogInformation("========== 权限验证结束（成功）==========");
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
