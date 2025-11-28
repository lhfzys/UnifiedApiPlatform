using FluentResults;

namespace UnifiedApiPlatform.Shared.Results;

// <summary>
/// FluentResults 扩展方法
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static Result<T> Success<T>(T value)
    {
        return Result.Ok(value);
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static Result<T> Failure<T>(string errorMessage)
    {
        return Result.Fail<T>(errorMessage);
    }

    /// <summary>
    /// 创建失败结果（带错误代码）
    /// </summary>
    public static Result<T> Failure<T>(string errorCode, string errorMessage)
    {
        return Result.Fail<T>(new Error(errorMessage).WithMetadata("ErrorCode", errorCode));
    }

    /// <summary>
    /// 获取错误代码
    /// </summary>
    public static string? GetErrorCode(this ResultBase result)
    {
        return result.Errors.FirstOrDefault()?.Metadata.TryGetValue("ErrorCode", out var code) == true
            ? code?.ToString()
            : null;
    }

    /// <summary>
    /// 获取所有错误消息
    /// </summary>
    public static List<string> GetErrorMessages(this ResultBase result)
    {
        return result.Errors.Select(e => e.Message).ToList();
    }
}
