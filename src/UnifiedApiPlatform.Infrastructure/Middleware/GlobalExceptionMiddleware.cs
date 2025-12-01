using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Shared.Constants;
using UnifiedApiPlatform.Shared.Helpers;
using UnifiedApiPlatform.Shared.Models;
using UnifiedApiPlatform.Shared.Resources;

namespace UnifiedApiPlatform.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发生未处理的异常");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var culture = CultureHelper.GetCurrentCulture(context);

        var response = ApiResponse.Fail(
            ErrorCodes.InternalServerError,
            ErrorMessages.GetMessage(ErrorCodes.InternalServerError, culture)
        );

        response.TraceId = context.TraceIdentifier;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
