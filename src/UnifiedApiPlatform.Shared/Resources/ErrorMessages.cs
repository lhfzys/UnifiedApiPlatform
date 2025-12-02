namespace UnifiedApiPlatform.Shared.Resources;

/// <summary>
/// 错误消息映射（支持多语言）
/// </summary>
public static class ErrorMessages
{
    private static readonly Dictionary<string, Dictionary<string, string>> Messages = new()
    {
        // 中文消息
        ["zh-CN"] = new()
        {
            // 通用错误
            ["INTERNAL_SERVER_ERROR"] = "服务器内部错误，请稍后重试",
            ["VALIDATION_ERROR"] = "请求参数验证失败",
            ["NOT_FOUND"] = "请求的资源不存在",
            ["UNAUTHORIZED"] = "未授权，请先登录",
            ["FORBIDDEN"] = "没有权限访问此资源",
            ["BAD_REQUEST"] = "请求参数错误",
            ["CONFLICT"] = "资源冲突",

            // 用户相关
            ["USER_NOT_FOUND"] = "用户不存在",
            ["USER_INVALID_CREDENTIALS"] = "邮箱或密码错误",
            ["USER_ACCOUNT_INACTIVE"] = "账户未激活，请联系管理员",
            ["USER_ACCOUNT_LOCKED"] = "账户已被锁定，请稍后重试",
            ["USER_EMAIL_ALREADY_EXISTS"] = "该邮箱已被注册",
            ["USER_USERNAME_ALREADY_EXISTS"] = "该用户名已被使用",
            ["USER_PASSWORD_TOO_WEAK"] = "密码强度不足",
            ["USER_OLD_PASSWORD_INCORRECT"] = "原密码错误",

            // 认证相关
            ["TOKEN_INVALID"] = "无效的访问令牌",
            ["TOKEN_EXPIRED"] = "访问令牌已过期",
            ["REFRESH_TOKEN_INVALID"] = "无效的刷新令牌",
            ["REFRESH_TOKEN_EXPIRED"] = "刷新令牌已过期",

            // 租户相关
            ["TENANT_NOT_FOUND"] = "租户不存在",
            ["TENANT_INACTIVE"] = "租户未激活",
            ["TENANT_SUSPENDED"] = "租户已被暂停",
            ["TENANT_IDENTIFIER_ALREADY_EXISTS"] = "租户标识符已存在",

            // 角色权限
            ["ROLE_NOT_FOUND"] = "角色不存在",
            ["ROLE_NAME_ALREADY_EXISTS"] = "角色名称已存在",
            ["ROLE_CANNOT_DELETE_SYSTEM_ROLE"] = "不能删除系统角色",
            ["PERMISSION_NOT_FOUND"] = "权限不存在",

            // 文件相关
            ["FILE_NOT_FOUND"] = "文件不存在",
            ["FILE_SIZE_TOO_LARGE"] = "文件大小超出限制",
            ["FILE_TYPE_NOT_ALLOWED"] = "不支持的文件类型",
            ["FILE_UPLOAD_FAILED"] = "文件上传失败",

            ["UNAUTHORIZED"] = "未授权，请先登录",
            ["FORBIDDEN"] = "没有权限访问此资源",
            ["PERMISSION_DENIED"] = "权限不足",
        },

        // 英文消息
        ["en-US"] = new()
        {
            // 通用错误
            ["INTERNAL_SERVER_ERROR"] = "Internal server error, please try again later",
            ["VALIDATION_ERROR"] = "Request validation failed",
            ["NOT_FOUND"] = "The requested resource was not found",
            ["UNAUTHORIZED"] = "Unauthorized, please login first",
            ["FORBIDDEN"] = "You don't have permission to access this resource",
            ["BAD_REQUEST"] = "Invalid request parameters",
            ["CONFLICT"] = "Resource conflict",

            // 用户相关
            ["USER_NOT_FOUND"] = "User not found",
            ["USER_INVALID_CREDENTIALS"] = "Invalid email or password",
            ["USER_ACCOUNT_INACTIVE"] = "Account is inactive, please contact administrator",
            ["USER_ACCOUNT_LOCKED"] = "Account is locked, please try again later",
            ["USER_EMAIL_ALREADY_EXISTS"] = "This email is already registered",
            ["USER_USERNAME_ALREADY_EXISTS"] = "This username is already taken",
            ["USER_PASSWORD_TOO_WEAK"] = "Password is too weak",
            ["USER_OLD_PASSWORD_INCORRECT"] = "Old password is incorrect",

            // 认证相关
            ["TOKEN_INVALID"] = "Invalid access token",
            ["TOKEN_EXPIRED"] = "Access token has expired",
            ["REFRESH_TOKEN_INVALID"] = "Invalid refresh token",
            ["REFRESH_TOKEN_EXPIRED"] = "Refresh token has expired",

            // 租户相关
            ["TENANT_NOT_FOUND"] = "Tenant not found",
            ["TENANT_INACTIVE"] = "Tenant is inactive",
            ["TENANT_SUSPENDED"] = "Tenant has been suspended",
            ["TENANT_IDENTIFIER_ALREADY_EXISTS"] = "Tenant identifier already exists",

            // 角色权限
            ["ROLE_NOT_FOUND"] = "Role not found",
            ["ROLE_NAME_ALREADY_EXISTS"] = "Role name already exists",
            ["ROLE_CANNOT_DELETE_SYSTEM_ROLE"] = "Cannot delete system role",
            ["PERMISSION_NOT_FOUND"] = "Permission not found",

            // 文件相关
            ["FILE_NOT_FOUND"] = "File not found",
            ["FILE_SIZE_TOO_LARGE"] = "File size exceeds limit",
            ["FILE_TYPE_NOT_ALLOWED"] = "File type not allowed",
            ["FILE_UPLOAD_FAILED"] = "File upload failed",

            ["UNAUTHORIZED"] = "Unauthorized, please login first",
            ["FORBIDDEN"] = "You don't have permission to access this resource",
            ["PERMISSION_DENIED"] = "Permission denied",
        }
    };

    /// <summary>
    /// 获取错误消息
    /// </summary>
    public static string GetMessage(string errorCode, string? culture = null)
    {
        culture ??= "zh-CN"; // 默认中文

        if (Messages.TryGetValue(culture, out var cultureMessages) &&
            cultureMessages.TryGetValue(errorCode, out var message))
        {
            return message;
        }

        // 回退到英文
        if (Messages.TryGetValue("en-US", out var enMessages) &&
            enMessages.TryGetValue(errorCode, out var enMessage))
        {
            return enMessage;
        }

        // 最后回退
        return errorCode;
    }
}
