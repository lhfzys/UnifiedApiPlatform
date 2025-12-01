namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 缓存键常量
/// </summary>
public static class CacheKeys
{
    // 用户相关
    public const string UserPermissions = "User:Permissions:{0}:{1}"; // TenantId:UserId
    public const string UserMenus = "User:Menus:{0}:{1}";
    public const string UserInfo = "User:Info:{0}:{1}";

    // 角色相关
    public const string RolePermissions = "Role:Permissions:{0}:{1}"; // TenantId:RoleId
    public const string RoleMenus = "Role:Menus:{0}:{1}";

    // 字典相关
    public const string Dictionary = "Dictionary:{0}:{1}"; // TenantId:CategoryCode
    public const string DictionaryCategory = "Dictionary:Category:{0}"; // TenantId

    // 配置相关
    public const string SystemSettings = "System:Settings";
    public const string TenantSettings = "Tenant:Settings:{0}"; // TenantId

    // 租户相关
    public const string TenantInfo = "Tenant:Info:{0}"; // TenantId

    // 幂等性
    public const string IdempotencyKey = "Idempotency:{0}"; // IdempotencyKey
}
