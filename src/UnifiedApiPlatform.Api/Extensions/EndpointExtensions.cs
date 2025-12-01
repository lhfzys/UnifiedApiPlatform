using FastEndpoints;
using FluentResults;
using UnifiedApiPlatform.Shared.Helpers;
using UnifiedApiPlatform.Shared.Models;
using UnifiedApiPlatform.Shared.Resources;

namespace UnifiedApiPlatform.Api.Extensions;

public static class EndpointExtensions
{
    /// <summary>
    /// 发送成功响应
    /// </summary>
    public static async Task SendOkAsync<T>(this IEndpoint endpoint, T data, CancellationToken ct = default)
    {
        var response = ApiResponse<T>.Ok(data);
        await endpoint.HttpContext.Response.SendAsync(response, cancellation: ct);
    }

    /// <summary>
    /// 发送成功响应（无数据）
    /// </summary>
    public static async Task SendOkAsync(this IEndpoint endpoint, string? message = null, CancellationToken ct = default)
    {
        var response = ApiResponse.Ok(message);
        await endpoint.HttpContext.Response.SendAsync(response, cancellation: ct);
    }

    /// <summary>
    /// 发送失败响应
    /// </summary>
    public static async Task SendErrorAsync(this IEndpoint endpoint, string errorCode, int statusCode = 400, CancellationToken ct = default)
    {
        // 使用 CultureHelper 解析语言
        var culture = CultureHelper.GetCurrentCulture(endpoint.HttpContext);
        var message = ErrorMessages.GetMessage(errorCode, culture);

        var response = ApiResponse.Fail(errorCode, message);
        response.TraceId = endpoint.HttpContext.TraceIdentifier;

        await endpoint.HttpContext.Response.SendAsync(response, statusCode, cancellation: ct);
    }

    /// <summary>
    /// 处理 FluentResults 的 Result
    /// </summary>
    public static async Task SendResultAsync<T>(this IEndpoint endpoint, Result<T> result, CancellationToken ct = default)
    {
        if (result.IsSuccess)
        {
            await endpoint.SendOkAsync(result.Value, ct);
        }
        else
        {
            var firstError = result.Errors.First();
            await endpoint.SendErrorAsync(firstError.Message, ct: ct);
        }
    }
}
