namespace UnifiedApiPlatform.Shared.Models;

/// <summary>
/// 统一 API 响应
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误码（成功时为 null）
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 错误消息（成功时为 null）
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 追踪 ID（用于日志关联）
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// 验证错误（字段级错误）
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse<T> Fail(string errorCode, string message, Dictionary<string, string[]>? validationErrors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
            ValidationErrors = validationErrors
        };
    }
}

/// <summary>
/// 无数据的统一响应
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public new static ApiResponse Fail(string errorCode, string message, Dictionary<string, string[]>? validationErrors = null)
    {
        return new ApiResponse
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
            ValidationErrors = validationErrors
        };
    }
}
