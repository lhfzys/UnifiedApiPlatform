using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Application.Common.Interfaces;

/// <summary>
/// 审计日志服务接口
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// 记录操作日志
    /// </summary>
    Task LogOperationAsync(
        string action,
        string httpMethod,
        string requestPath,
        string? requestBody,
        int statusCode,
        string? responseBody,
        long duration,
        string ipAddress,
        string? userAgent,
        string? entityType = null,
        string? entityId = null,
        string? exception = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录登录日志
    /// </summary>
    Task LogLoginAsync(
        string userName,
        string loginType,
        bool isSuccess,
        string ipAddress,
        string? userAgent,
        string? failureReason = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
}
