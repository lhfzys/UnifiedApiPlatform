namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 错误代码常量
/// </summary>
public static class ErrorCodes
{
    // ==================== 通用错误 (1000-1999) ====================
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string BadRequest = "BAD_REQUEST";
    public const string Conflict = "CONFLICT";

    // ==================== 用户相关 (2000-2999) ====================
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string UserInvalidCredentials = "USER_INVALID_CREDENTIALS";
    public const string UserAccountInactive = "USER_ACCOUNT_INACTIVE";
    public const string UserAccountLocked = "USER_ACCOUNT_LOCKED";
    public const string UserEmailAlreadyExists = "USER_EMAIL_ALREADY_EXISTS";
    public const string UserUsernameAlreadyExists = "USER_USERNAME_ALREADY_EXISTS";
    public const string UserPasswordTooWeak = "USER_PASSWORD_TOO_WEAK";
    public const string UserOldPasswordIncorrect = "USER_OLD_PASSWORD_INCORRECT";

    // ==================== 认证相关 (3000-3999) ====================
    public const string TokenInvalid = "TOKEN_INVALID";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string RefreshTokenInvalid = "REFRESH_TOKEN_INVALID";
    public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";

    // ==================== 租户相关 (4000-4999) ====================
    public const string TenantNotFound = "TENANT_NOT_FOUND";
    public const string TenantInactive = "TENANT_INACTIVE";
    public const string TenantSuspended = "TENANT_SUSPENDED";
    public const string TenantIdentifierAlreadyExists = "TENANT_IDENTIFIER_ALREADY_EXISTS";

    // ==================== 角色权限相关 (5000-5999) ====================
    public const string RoleNotFound = "ROLE_NOT_FOUND";
    public const string RoleNameAlreadyExists = "ROLE_NAME_ALREADY_EXISTS";
    public const string RoleCannotDeleteSystemRole = "ROLE_CANNOT_DELETE_SYSTEM_ROLE";
    public const string PermissionNotFound = "PERMISSION_NOT_FOUND";
    public const string PermissionDenied = "PERMISSION_DENIED";

    // ==================== 文件相关 (6000-6999) ====================
    public const string FileNotFound = "FILE_NOT_FOUND";
    public const string FileSizeTooLarge = "FILE_SIZE_TOO_LARGE";
    public const string FileTypeNotAllowed = "FILE_TYPE_NOT_ALLOWED";
    public const string FileUploadFailed = "FILE_UPLOAD_FAILED";

    // ==================== 业务相关 (7000+) ====================
}
