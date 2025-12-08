using System.Globalization;

namespace UnifiedApiPlatform.Shared.Resources;

/// <summary>
/// 错误消息资源
/// </summary>
public static class ErrorMessages
{
    private static readonly Dictionary<string, Dictionary<string, string>> Messages = new()
    {
        // ==================== 中文消息 ====================
        ["zh-CN"] = new()
        {
            // 通用错误
            ["INTERNAL_SERVER_ERROR"] = "服务器内部错误",
            ["VALIDATION_ERROR"] = "请求参数验证失败",
            ["NOT_FOUND"] = "资源不存在",
            ["UNAUTHORIZED"] = "未授权，请先登录",
            ["FORBIDDEN"] = "没有权限访问此资源",
            ["BAD_REQUEST"] = "请求参数错误",
            ["CONFLICT"] = "资源冲突",
            ["OPERATION_FAILED"] = "操作失败",

            // 用户相关
            ["USER_NOT_FOUND"] = "用户不存在",
            ["USER_INVALID_CREDENTIALS"] = "邮箱或密码错误",
            ["USER_ACCOUNT_INACTIVE"] = "账户未激活",
            ["USER_ACCOUNT_LOCKED"] = "账户已被锁定",
            ["USER_EMAIL_ALREADY_EXISTS"] = "邮箱已被使用",
            ["USER_USERNAME_ALREADY_EXISTS"] = "用户名已存在",
            ["USER_PASSWORD_TOO_WEAK"] = "密码强度不足",
            ["USER_OLD_PASSWORD_INCORRECT"] = "原密码不正确",

            // 认证相关
            ["TOKEN_INVALID"] = "无效的令牌",
            ["TOKEN_EXPIRED"] = "令牌已过期",
            ["REFRESH_TOKEN_INVALID"] = "无效的刷新令牌",
            ["REFRESH_TOKEN_EXPIRED"] = "刷新令牌已过期",
            ["REFRESH_TOKEN_NOT_FOUND"] = "刷新令牌不存在",

            // 租户相关
            ["TENANT_NOT_FOUND"] = "租户不存在",
            ["TENANT_INACTIVE"] = "租户未激活",
            ["TENANT_SUSPENDED"] = "租户已暂停",
            ["TENANT_IDENTIFIER_ALREADY_EXISTS"] = "租户标识已存在",

            // 角色权限相关
            ["ROLE_NOT_FOUND"] = "角色不存在",
            ["ROLE_NAME_ALREADY_EXISTS"] = "角色名称已存在",
            ["ROLE_CANNOT_DELETE_SYSTEM_ROLE"] = "不能删除系统角色",
            ["PERMISSION_NOT_FOUND"] = "权限不存在",
            ["PERMISSION_DENIED"] = "权限不足",

            // 文件相关
            ["FILE_NOT_FOUND"] = "文件不存在",
            ["FILE_SIZE_TOO_LARGE"] = "文件过大",
            ["FILE_TYPE_NOT_ALLOWED"] = "文件类型不允许",
            ["FILE_UPLOAD_FAILED"] = "文件上传失败",

            // 组织相关
            ["ORGANIZATION_NOT_FOUND"] = "组织不存在",
            ["ORGANIZATION_NAME_ALREADY_EXISTS"] = "组织名称已存在",
            ["ORGANIZATION_CANNOT_DELETE_WITH_MEMBERS"] = "不能删除有成员的组织",

            // 菜单相关
            ["MENU_NOT_FOUND"] = "菜单不存在",
            ["MENU_CODE_ALREADY_EXISTS"] = "菜单代码已存在",
            ["MENU_CANNOT_DELETE_WITH_CHILDREN"] = "不能删除有子菜单的菜单",

            // 业务相关
            ["DUPLICATE_DATA"] = "数据重复",
            ["DATA_NOT_FOUND"] = "数据不存在",
            ["DATA_ALREADY_DELETED"] = "数据已被删除",
            ["DATA_IN_USE"] = "数据正在使用中，无法删除",
        },

        // ==================== 英文消息 ====================
        ["en-US"] = new()
        {
            // 通用错误
            ["INTERNAL_SERVER_ERROR"] = "Internal server error",
            ["VALIDATION_ERROR"] = "Validation failed",
            ["NOT_FOUND"] = "Resource not found",
            ["UNAUTHORIZED"] = "Unauthorized, please login first",
            ["FORBIDDEN"] = "You don't have permission to access this resource",
            ["BAD_REQUEST"] = "Bad request",
            ["CONFLICT"] = "Resource conflict",
            ["OPERATION_FAILED"] = "Operation failed",

            // 用户相关
            ["USER_NOT_FOUND"] = "User not found",
            ["USER_INVALID_CREDENTIALS"] = "Invalid email or password",
            ["USER_ACCOUNT_INACTIVE"] = "Account is not active",
            ["USER_ACCOUNT_LOCKED"] = "Account is locked",
            ["USER_EMAIL_ALREADY_EXISTS"] = "Email already exists",
            ["USER_USERNAME_ALREADY_EXISTS"] = "Username already exists",
            ["USER_PASSWORD_TOO_WEAK"] = "Password is too weak",
            ["USER_OLD_PASSWORD_INCORRECT"] = "Old password is incorrect",

            // 认证相关
            ["TOKEN_INVALID"] = "Invalid token",
            ["TOKEN_EXPIRED"] = "Token has expired",
            ["REFRESH_TOKEN_INVALID"] = "Invalid refresh token",
            ["REFRESH_TOKEN_EXPIRED"] = "Refresh token has expired",
            ["REFRESH_TOKEN_NOT_FOUND"] = "Refresh token not found",

            // 租户相关
            ["TENANT_NOT_FOUND"] = "Tenant not found",
            ["TENANT_INACTIVE"] = "Tenant is not active",
            ["TENANT_SUSPENDED"] = "Tenant has been suspended",
            ["TENANT_IDENTIFIER_ALREADY_EXISTS"] = "Tenant identifier already exists",

            // 角色权限相关
            ["ROLE_NOT_FOUND"] = "Role not found",
            ["ROLE_NAME_ALREADY_EXISTS"] = "Role name already exists",
            ["ROLE_CANNOT_DELETE_SYSTEM_ROLE"] = "Cannot delete system role",
            ["PERMISSION_NOT_FOUND"] = "Permission not found",
            ["PERMISSION_DENIED"] = "Permission denied",

            // 文件相关
            ["FILE_NOT_FOUND"] = "File not found",
            ["FILE_SIZE_TOO_LARGE"] = "File is too large",
            ["FILE_TYPE_NOT_ALLOWED"] = "File type not allowed",
            ["FILE_UPLOAD_FAILED"] = "File upload failed",

            // 组织相关
            ["ORGANIZATION_NOT_FOUND"] = "Organization not found",
            ["ORGANIZATION_NAME_ALREADY_EXISTS"] = "Organization name already exists",
            ["ORGANIZATION_CANNOT_DELETE_WITH_MEMBERS"] = "Cannot delete organization with members",

            // 菜单相关
            ["MENU_NOT_FOUND"] = "Menu not found",
            ["MENU_CODE_ALREADY_EXISTS"] = "Menu code already exists",
            ["MENU_CANNOT_DELETE_WITH_CHILDREN"] = "Cannot delete menu with children",

            // 业务相关
            ["DUPLICATE_DATA"] = "Duplicate data",
            ["DATA_NOT_FOUND"] = "Data not found",
            ["DATA_ALREADY_DELETED"] = "Data has been deleted",
            ["DATA_IN_USE"] = "Data is in use and cannot be deleted",
        }
    };

    /// <summary>
    /// 获取错误消息
    /// </summary>
    public static string GetMessage(string errorCode, CultureInfo culture)
    {
        var cultureName = culture.Name;

        // 尝试获取指定语言的消息
        if (Messages.TryGetValue(cultureName, out var messages) &&
            messages.TryGetValue(errorCode, out var message))
        {
            return message;
        }

        // 尝试获取英文消息
        if (Messages.TryGetValue("en-US", out var enMessages) &&
            enMessages.TryGetValue(errorCode, out var enMessage))
        {
            return enMessage;
        }

        // 返回错误码本身
        return errorCode;
    }

    /// <summary>
    /// 获取错误消息（使用当前文化）
    /// </summary>
    public static string GetMessage(string errorCode)
    {
        return GetMessage(errorCode, CultureInfo.CurrentCulture);
    }
}
