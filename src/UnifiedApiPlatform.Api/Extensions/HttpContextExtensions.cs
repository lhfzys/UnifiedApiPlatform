using System.Text.Json;
using UnifiedApiPlatform.Shared.Helpers;
using UnifiedApiPlatform.Shared.Models;
using UnifiedApiPlatform.Shared.Resources;

namespace UnifiedApiPlatform.Api.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// 发送错误响应
    /// </summary>
    public static async Task SendErrorResponseAsync(
        this HttpContext context,
        string errorCode,
        Dictionary<string, string[]>? validationErrors = null,
        int statusCode = 400,
        CancellationToken ct = default)
    {
        // 获取当前语言
        var culture = CultureHelper.GetCurrentCulture(context);
        var message = ErrorMessages.GetMessage(errorCode, culture);

        // 构建统一响应
        var response = ApiResponse.Fail(errorCode, message, validationErrors);
        response.TraceId = context.TraceIdentifier;

        // 设置响应状态码和内容类型
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        // 序列化并发送响应
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json, ct);
    }
}
