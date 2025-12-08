using FastEndpoints;
using FluentResults;
using UnifiedApiPlatform.Shared.Constants;
using UnifiedApiPlatform.Shared.Helpers;
using UnifiedApiPlatform.Shared.Models;
using UnifiedApiPlatform.Shared.Resources;

namespace UnifiedApiPlatform.Api.Extensions;

public static class EndpointExtensions
{
    /// <summary>
    /// 发送 Result 响应（有数据）
    /// </summary>
    public static async Task SendResultAsync<T>(this IEndpoint endpoint, Result<T> result,
        CancellationToken ct = default)
    {
        if (result.IsSuccess)
        {
            await endpoint.SendOkAsync(result.Value, ct);
        }
        else
        {
            var firstError = result.Errors.First();

            // 检查是否是验证错误
            if (firstError.Message == "VALIDATION_ERROR" &&
                firstError.Metadata.TryGetValue("ValidationErrors", out var validationErrorsObj))
            {
                var validationErrors = validationErrorsObj as Dictionary<string, string[]>;
                await endpoint.SendValidationErrorAsync(validationErrors, ct);
            }
            else
            {
                // 普通错误（自定义消息）
                await endpoint.SendBusinessErrorAsync(firstError.Message, ct: ct);
            }
        }
    }

    /// <summary>
    /// 发送 Result 响应（无数据）
    /// </summary>
    public static async Task SendResultAsync(this IEndpoint endpoint, Result result,
        CancellationToken ct = default)
    {
        if (result.IsSuccess)
        {
            await endpoint.SendOkAsync(ct);
        }
        else
        {
            var firstError = result.Errors.First();

            // 检查是否是验证错误
            if (firstError.Message == "VALIDATION_ERROR" &&
                firstError.Metadata.TryGetValue("ValidationErrors", out var validationErrorsObj))
            {
                var validationErrors = validationErrorsObj as Dictionary<string, string[]>;
                await endpoint.SendValidationErrorAsync(validationErrors, ct);
            }
            else
            {
                // 普通错误（自定义消息）
                await endpoint.SendBusinessErrorAsync(firstError.Message, ct: ct);
            }
        }
    }

    /// <summary>
    /// 发送 OK 响应（有数据）
    /// </summary>
    public static async Task SendOkAsync<T>(this IEndpoint endpoint, T data,
        CancellationToken ct = default)
    {
        var response = ApiResponse.Ok(data);
        response.TraceId = endpoint.HttpContext.TraceIdentifier;

        await endpoint.HttpContext.Response.WriteAsJsonAsync(response, ct);
    }

    /// <summary>
    /// 发送 OK 响应（无数据）
    /// </summary>
    public static async Task SendOkAsync(this IEndpoint endpoint, CancellationToken ct = default)
    {
        var response = ApiResponse.Ok();
        response.TraceId = endpoint.HttpContext.TraceIdentifier;

        await endpoint.HttpContext.Response.WriteAsJsonAsync(response, ct);
    }

    /// <summary>
    /// 发送验证错误响应
    /// </summary>
    public static async Task SendValidationErrorAsync(this IEndpoint endpoint,
        Dictionary<string, string[]>? validationErrors,
        CancellationToken ct = default)
    {
        var culture = CultureHelper.GetCurrentCulture(endpoint.HttpContext);
        var message = ErrorMessages.GetMessage(ErrorCodes.ValidationError);

        var response = ApiResponse.Fail(ErrorCodes.ValidationError, message, validationErrors);
        response.TraceId = endpoint.HttpContext.TraceIdentifier;

        endpoint.HttpContext.Response.StatusCode = 400;
        await endpoint.HttpContext.Response.WriteAsJsonAsync(response, ct);
    }

    /// <summary>
    /// 发送业务错误响应（使用错误码）
    /// </summary>
    public static async Task SendErrorAsync(this IEndpoint endpoint, string errorCode,
        int statusCode = 400,
        CancellationToken ct = default)
    {
        var culture = CultureHelper.GetCurrentCulture(endpoint.HttpContext);
        var message = ErrorMessages.GetMessage(errorCode);

        var response = ApiResponse.Fail(errorCode, message);
        response.TraceId = endpoint.HttpContext.TraceIdentifier;

        endpoint.HttpContext.Response.StatusCode = statusCode;
        await endpoint.HttpContext.Response.WriteAsJsonAsync(response, ct);
    }

    /// <summary>
    /// 发送业务错误响应（自定义消息）
    /// </summary>
    public static async Task SendBusinessErrorAsync(this IEndpoint endpoint, string message,
        int statusCode = 400,
        CancellationToken ct = default)
    {
        var response = ApiResponse.Fail(ErrorCodes.OperationFailed, message);
        response.TraceId = endpoint.HttpContext.TraceIdentifier;

        endpoint.HttpContext.Response.StatusCode = statusCode;
        await endpoint.HttpContext.Response.WriteAsJsonAsync(response, ct);
    }
}
