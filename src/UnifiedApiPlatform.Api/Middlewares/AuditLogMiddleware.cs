using System.Diagnostics;
using System.Text;
using System.Text.Json;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Api.Middlewares;

/// <summary>
/// 审计日志中间件（简化版 - 不拦截响应体）
/// </summary>
public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;

    public AuditLogMiddleware(
        RequestDelegate next,
        ILogger<AuditLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuditLogService auditLogService,
        ICurrentUserService currentUser)
    {
        // 跳过不需要审计的请求
        if (ShouldSkipAudit(context))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path.Value ?? string.Empty;
        var httpMethod = context.Request.Method;

        // 读取请求体
        var requestBody = await ReadRequestBodyAsync(context);

        // ✅ 直接执行下一个中间件，不拦截响应流
        await _next(context);

        stopwatch.Stop();

        // ✅ 在请求完成后记录审计日志
        await LogAuditAsync(
            auditLogService,
            currentUser,
            context,
            httpMethod,
            requestPath,
            requestBody,
            context.Response.StatusCode,
            null, // 不记录成功响应体，减少存储压力
            stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// 判断是否跳过审计
    /// </summary>
    private static bool ShouldSkipAudit(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

        // 跳过健康检查
        if (path.Contains("/health") || path.Contains("/ping"))
            return true;

        // 跳过 Swagger
        if (path.Contains("/swagger") || path.Contains("/api-docs"))
            return true;

        // 跳过静态文件
        if (path.Contains("/favicon.ico") || path.Contains("/robots.txt"))
            return true;

        // 跳过 GET 请求的审计日志查询（避免递归）
        if (context.Request.Method == "GET" && path.Contains("/audit-logs"))
            return true;

        return false;
    }

    /// <summary>
    /// 读取请求体
    /// </summary>
    private static async Task<string?> ReadRequestBodyAsync(HttpContext context)
    {
        try
        {
            // 只记录 POST/PUT/PATCH 的请求体
            if (context.Request.Method != "POST" &&
                context.Request.Method != "PUT" &&
                context.Request.Method != "PATCH")
            {
                return null;
            }

            // 检查 Content-Type
            var contentType = context.Request.ContentType?.ToLower() ?? string.Empty;
            if (!contentType.Contains("application/json") &&
                !contentType.Contains("application/x-www-form-urlencoded"))
            {
                return null;
            }

            // 启用缓冲
            context.Request.EnableBuffering();

            // 读取请求体
            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();

            // 重置流位置
            context.Request.Body.Position = 0;

            // 过滤敏感信息
            return FilterSensitiveData(body);
        }
        catch (Exception ex)
        {
           // _logger.LogWarning(ex, "读取请求体失败");
            return null;
        }
    }

    /// <summary>
    /// 过滤敏感数据
    /// </summary>
    private static string? FilterSensitiveData(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return json;

        try
        {
            // 解析 JSON
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            // 敏感字段列表
            var sensitiveFields = new[]
            {
                "password",
                "oldPassword",
                "newPassword",
                "confirmPassword",
                "secret",
                "token",
                "accessToken",
                "refreshToken",
                "apiKey",
                "privateKey",
                "creditCard",
                "cvv"
            };

            // 递归过滤
            var filtered = FilterJsonElement(root, sensitiveFields);

            return JsonSerializer.Serialize(filtered, new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch
        {
            return json;
        }
    }

    /// <summary>
    /// 递归过滤 JSON 元素
    /// </summary>
    private static object? FilterJsonElement(JsonElement element, string[] sensitiveFields)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();
                foreach (var property in element.EnumerateObject())
                {
                    var key = property.Name;
                    var isSensitive = sensitiveFields.Any(f =>
                        key.Equals(f, StringComparison.OrdinalIgnoreCase));

                    dict[key] = isSensitive
                        ? "***FILTERED***"
                        : FilterJsonElement(property.Value, sensitiveFields);
                }
                return dict;

            case JsonValueKind.Array:
                return element.EnumerateArray()
                    .Select(e => FilterJsonElement(e, sensitiveFields))
                    .ToList();

            case JsonValueKind.String:
                return element.GetString();

            case JsonValueKind.Number:
                return element.TryGetInt64(out var l) ? l : element.GetDouble();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
                return null;

            default:
                return element.ToString();
        }
    }

    /// <summary>
    /// 记录审计日志
    /// </summary>
    private async Task LogAuditAsync(
        IAuditLogService auditLogService,
        ICurrentUserService currentUser,
        HttpContext context,
        string httpMethod,
        string requestPath,
        string? requestBody,
        int statusCode,
        string? responseBody,
        long duration,
        string? exception = null)
    {
        try
        {
            // 确定操作类型
            var action = DetermineAction(httpMethod, requestPath);

            // 获取实体信息
            var (entityType, entityId) = ExtractEntityInfo(requestPath);

            // 获取 IP 地址
            var ipAddress = GetIpAddress(context);

            // 获取 User-Agent
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();

            // 记录审计日志
            await auditLogService.LogOperationAsync(
                action,
                httpMethod,
                requestPath,
                requestBody,
                statusCode,
                responseBody,
                duration,
                ipAddress,
                userAgent,
                entityType,
                entityId,
                exception);
        }
        catch (Exception ex)
        {
            // 审计日志记录失败不应该影响主流程
            _logger.LogError(ex, "记录审计日志失败");
        }
    }

    /// <summary>
    /// 确定操作类型
    /// </summary>
    private static string DetermineAction(string httpMethod, string requestPath)
    {
        var path = requestPath.ToLower();

        // 登录相关
        if (path.Contains("/auth/login"))
            return AuditAction.Login;

        if (path.Contains("/auth/logout"))
            return AuditAction.Logout;

        if (path.Contains("/auth/refresh-token"))
            return AuditAction.RefreshToken;

        if (path.Contains("/change-password"))
            return AuditAction.ChangePassword;

        // 导入导出
        if (path.Contains("/export"))
            return AuditAction.Export;

        if (path.Contains("/import"))
            return AuditAction.Import;

        // CRUD 操作
        return httpMethod switch
        {
            "GET" => AuditAction.Query,
            "POST" => AuditAction.Create,
            "PUT" or "PATCH" => AuditAction.Update,
            "DELETE" => AuditAction.Delete,
            _ => "Unknown"
        };
    }

    /// <summary>
    /// 提取实体信息
    /// </summary>
    private static (string? EntityType, string? EntityId) ExtractEntityInfo(string requestPath)
    {
        try
        {
            // 示例：/api/v1/users/123 -> EntityType: User, EntityId: 123
            var segments = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 3)
                return (null, null);

            // 找到 /api/v1/ 或 /api/ 后的第一个段作为 EntityType
            int entityTypeIndex = -1;

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == "v1" || segments[i] == "api")
                {
                    entityTypeIndex = i + 1;
                    break;
                }
            }

            if (entityTypeIndex == -1 || entityTypeIndex >= segments.Length)
                return (null, null);

            var entityType = segments[entityTypeIndex];

            // 如果下一个段是 GUID，则作为 EntityId
            string? entityId = null;
            if (entityTypeIndex + 1 < segments.Length)
            {
                var nextSegment = segments[entityTypeIndex + 1];
                if (Guid.TryParse(nextSegment, out _))
                {
                    entityId = nextSegment;
                }
            }

            // 首字母大写
            if (!string.IsNullOrEmpty(entityType))
            {
                entityType = char.ToUpper(entityType[0]) + entityType[1..];

                // 移除复数形式（简单处理）
                if (entityType.EndsWith("s") && entityType.Length > 1)
                {
                    entityType = entityType[..^1];
                }
            }

            return (entityType, entityId);
        }
        catch
        {
            return (null, null);
        }
    }

    /// <summary>
    /// 获取 IP 地址
    /// </summary>
    private static string GetIpAddress(HttpContext context)
    {
        // 尝试从 X-Forwarded-For 获取（代理/负载均衡）
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',');
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // 尝试从 X-Real-IP 获取
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // 从连接信息获取
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
