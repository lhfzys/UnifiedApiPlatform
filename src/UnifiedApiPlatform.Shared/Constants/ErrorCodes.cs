namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 错误码常量
/// 命名规范：[模块]_[错误类型]，全大写，下划线分隔
/// 分类范围：
/// - 1000-1999: 通用错误
/// - 2000-2999: 用户相关
/// - 3000-3999: 认证相关
/// - 4000-4999: 租户相关
/// - 5000-5999: 角色权限相关
/// - 6000-6999: 文件相关
/// - 7000-7999: 组织相关
/// - 8000-8999: 菜单相关
/// - 9000+: 业务相关
/// </summary>
public static class ErrorCodes
{
    // ==================== 通用错误 (1000-1999) ====================
    /// <summary>服务器内部错误</summary>
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";

    /// <summary>请求参数验证失败</summary>
    public const string ValidationError = "VALIDATION_ERROR";

    /// <summary>资源不存在</summary>
    public const string NotFound = "NOT_FOUND";

    /// <summary>未授权，请先登录</summary>
    public const string Unauthorized = "UNAUTHORIZED";

    /// <summary>没有权限访问</summary>
    public const string Forbidden = "FORBIDDEN";

    /// <summary>请求参数错误</summary>
    public const string BadRequest = "BAD_REQUEST";

    /// <summary>资源冲突</summary>
    public const string Conflict = "CONFLICT";

    /// <summary>操作失败</summary>
    public const string OperationFailed = "OPERATION_FAILED";

    // ==================== 用户相关 (2000-2999) ====================
    /// <summary>用户不存在</summary>
    public const string UserNotFound = "USER_NOT_FOUND";

    /// <summary>邮箱或密码错误</summary>
    public const string UserInvalidCredentials = "USER_INVALID_CREDENTIALS";

    /// <summary>账户未激活</summary>
    public const string UserAccountInactive = "USER_ACCOUNT_INACTIVE";

    /// <summary>账户已被锁定</summary>
    public const string UserAccountLocked = "USER_ACCOUNT_LOCKED";

    /// <summary>邮箱已被使用</summary>
    public const string UserEmailAlreadyExists = "USER_EMAIL_ALREADY_EXISTS";

    /// <summary>用户名已存在</summary>
    public const string UserUsernameAlreadyExists = "USER_USERNAME_ALREADY_EXISTS";

    /// <summary>密码强度不足</summary>
    public const string UserPasswordTooWeak = "USER_PASSWORD_TOO_WEAK";

    /// <summary>原密码不正确</summary>
    public const string UserOldPasswordIncorrect = "USER_OLD_PASSWORD_INCORRECT";

    // ==================== 认证相关 (3000-3999) ====================
    /// <summary>无效的令牌</summary>
    public const string TokenInvalid = "TOKEN_INVALID";

    /// <summary>令牌已过期</summary>
    public const string TokenExpired = "TOKEN_EXPIRED";

    /// <summary>无效的刷新令牌</summary>
    public const string RefreshTokenInvalid = "REFRESH_TOKEN_INVALID";

    /// <summary>刷新令牌已过期</summary>
    public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";

    /// <summary>刷新令牌不存在</summary>
    public const string RefreshTokenNotFound = "REFRESH_TOKEN_NOT_FOUND";

    // ==================== 租户相关 (4000-4999) ====================
    /// <summary>租户不存在</summary>
    public const string TenantNotFound = "TENANT_NOT_FOUND";

    /// <summary>租户未激活</summary>
    public const string TenantInactive = "TENANT_INACTIVE";

    /// <summary>租户已暂停</summary>
    public const string TenantSuspended = "TENANT_SUSPENDED";

    /// <summary>租户标识已存在</summary>
    public const string TenantIdentifierAlreadyExists = "TENANT_IDENTIFIER_ALREADY_EXISTS";

    // ==================== 角色权限相关 (5000-5999) ====================
    /// <summary>角色不存在</summary>
    public const string RoleNotFound = "ROLE_NOT_FOUND";

    /// <summary>角色名称已存在</summary>
    public const string RoleNameAlreadyExists = "ROLE_NAME_ALREADY_EXISTS";

    /// <summary>不能删除系统角色</summary>
    public const string RoleCannotDeleteSystemRole = "ROLE_CANNOT_DELETE_SYSTEM_ROLE";

    /// <summary>权限不存在</summary>
    public const string PermissionNotFound = "PERMISSION_NOT_FOUND";

    /// <summary>权限不足</summary>
    public const string PermissionDenied = "PERMISSION_DENIED";

    // ==================== 文件相关 (6000-6999) ====================
    /// <summary>文件不存在</summary>
    public const string FileNotFound = "FILE_NOT_FOUND";

    /// <summary>文件过大</summary>
    public const string FileSizeTooLarge = "FILE_SIZE_TOO_LARGE";

    /// <summary>文件类型不允许</summary>
    public const string FileTypeNotAllowed = "FILE_TYPE_NOT_ALLOWED";

    /// <summary>文件上传失败</summary>
    public const string FileUploadFailed = "FILE_UPLOAD_FAILED";

    // ==================== 组织相关 (7000-7999) ====================
    // 7000-7999：组织相关
    public const string OrganizationNotFound = "ORGANIZATION_NOT_FOUND";
    public const string OrganizationCodeAlreadyExists = "ORGANIZATION_CODE_ALREADY_EXISTS";
    public const string OrganizationInUse = "ORGANIZATION_IN_USE";
    public const string OrganizationCannotDeleteHasChildren = "ORGANIZATION_CANNOT_DELETE_HAS_CHILDREN";
    public const string OrganizationCannotBeParentOfItself = "ORGANIZATION_CANNOT_BE_PARENT_OF_ITSELF";

    // ==================== 菜单相关 (8000-8999) ====================
    /// <summary>菜单不存在</summary>
    public const string MenuNotFound = "MENU_NOT_FOUND";

    /// <summary>菜单代码已存在</summary>
    public const string MenuCodeAlreadyExists = "MENU_CODE_ALREADY_EXISTS";

    /// <summary>不能删除有子菜单的菜单</summary>
    public const string MenuCannotDeleteWithChildren = "MENU_CANNOT_DELETE_WITH_CHILDREN";

    // ==================== 业务相关 (9000+) ====================
    /// <summary>数据重复</summary>
    public const string DuplicateData = "DUPLICATE_DATA";

    /// <summary>数据不存在</summary>
    public const string DataNotFound = "DATA_NOT_FOUND";

    /// <summary>数据已被删除</summary>
    public const string DataAlreadyDeleted = "DATA_ALREADY_DELETED";

    /// <summary>数据正在使用中，无法删除</summary>
    public const string DataInUse = "DATA_IN_USE";
}
