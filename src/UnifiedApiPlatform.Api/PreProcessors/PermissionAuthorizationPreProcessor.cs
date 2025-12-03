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


        // 获取用户的权限 Claims
        var userPermissions = ctx.HttpContext.User.Claims
            .Where(c => c.Type == CustomClaimTypes.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        logger.LogInformation("提取的权限列表 ({Count} 个): {Permissions}",
            userPermissions.Count,
            string.Join(", ", userPermissions));

        // 验证权限
        var hasPermission = permissionMetadata.RequireAll
            ? permissionMetadata.Permissions.All(p => userPermissions.Contains(p))
            : permissionMetadata.Permissions.Any(p => userPermissions.Contains(p));

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
            logger.LogInformation("✅ 权限验证通过");
        }

        logger.LogInformation("========== 权限验证结束 ==========");
    }
}

/// <summary>
/// 权限元数据
/// </summary>
public class PermissionMetadata
{
    public string[] Permissions { get; set; } = [];
    public bool RequireAll { get; set; } = false;
}
