namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 错误代码常量
/// </summary>
public static class ErrorCodes
{
    // 通用错误 (1000-1999)
    public const string ValidationError = "VALIDATION_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string BadRequest = "BAD_REQUEST";

    // 用户相关 (2000-2999)
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string UserEmailExists = "USER_EMAIL_EXISTS";
    public const string UserNameExists = "USER_NAME_EXISTS";
    public const string UserInvalidCredentials = "USER_INVALID_CREDENTIALS";
    public const string UserAccountLocked = "USER_ACCOUNT_LOCKED";
    public const string UserAccountInactive = "USER_ACCOUNT_INACTIVE";
    public const string UserPasswordInvalid = "USER_PASSWORD_INVALID";

    // 租户相关 (3000-3999)
    public const string TenantNotFound = "TENANT_NOT_FOUND";
    public const string TenantInactive = "TENANT_INACTIVE";
    public const string TenantSuspended = "TENANT_SUSPENDED";
    public const string TenantQuotaExceeded = "TENANT_QUOTA_EXCEEDED";
    public const string TenantIdentifierExists = "TENANT_IDENTIFIER_EXISTS";

    // 角色相关 (4000-4999)
    public const string RoleNotFound = "ROLE_NOT_FOUND";
    public const string RoleNameExists = "ROLE_NAME_EXISTS";
    public const string RoleInUse = "ROLE_IN_USE";
    public const string RoleSystemProtected = "ROLE_SYSTEM_PROTECTED";

    // 权限相关 (5000-5999)
    public const string PermissionNotFound = "PERMISSION_NOT_FOUND";
    public const string PermissionCodeExists = "PERMISSION_CODE_EXISTS";
    public const string PermissionSystemProtected = "PERMISSION_SYSTEM_PROTECTED";

    // 菜单相关 (6000-6999)
    public const string MenuNotFound = "MENU_NOT_FOUND";
    public const string MenuNameExists = "MENU_NAME_EXISTS";
    public const string MenuMaxDepthExceeded = "MENU_MAX_DEPTH_EXCEEDED";
    public const string MenuSystemProtected = "MENU_SYSTEM_PROTECTED";
    public const string MenuHasChildren = "MENU_HAS_CHILDREN";

    // 组织相关 (7000-7999)
    public const string OrganizationNotFound = "ORGANIZATION_NOT_FOUND";
    public const string OrganizationCodeExists = "ORGANIZATION_CODE_EXISTS";
    public const string OrganizationMaxDepthExceeded = "ORGANIZATION_MAX_DEPTH_EXCEEDED";
    public const string OrganizationHasChildren = "ORGANIZATION_HAS_CHILDREN";
    public const string OrganizationHasMembers = "ORGANIZATION_HAS_MEMBERS";

    // 文件相关 (8000-8999)
    public const string FileNotFound = "FILE_NOT_FOUND";
    public const string FileSizeExceeded = "FILE_SIZE_EXCEEDED";
    public const string FileTypeNotAllowed = "FILE_TYPE_NOT_ALLOWED";
    public const string FileUploadFailed = "FILE_UPLOAD_FAILED";

    // Token 相关 (9000-9999)
    public const string TokenInvalid = "TOKEN_INVALID";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string RefreshTokenInvalid = "REFRESH_TOKEN_INVALID";
    public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";
    public const string RefreshTokenRevoked = "REFRESH_TOKEN_REVOKED";
}
