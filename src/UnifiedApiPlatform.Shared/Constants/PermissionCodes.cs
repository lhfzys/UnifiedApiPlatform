namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 授权策略常量
/// </summary>
public static class PermissionCodes
{
    // ==================== 用户管理 ====================
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";

    // ==================== 角色管理 ====================
    public const string RolesView = "roles.view";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";

    // ==================== 权限管理 ====================
    public const string PermissionsView = "permissions.view";

    // ==================== 菜单管理 ====================
    public const string MenusView = "menus.view";
    public const string MenusCreate = "menus.create";
    public const string MenusUpdate = "menus.update";
    public const string MenusDelete = "menus.delete";

    // ==================== 组织管理 ====================
    public const string OrganizationsView = "organizations.view";
    public const string OrganizationsCreate = "organizations.create";
    public const string OrganizationsUpdate = "organizations.update";
    public const string OrganizationsDelete = "organizations.delete";

    // ==================== 文件管理 ====================
    public const string FilesView = "files.view";
    public const string FilesUpload = "files.upload";
    public const string FilesDelete = "files.delete";

    // ==================== 系统管理 ====================
    public const string SystemSettings = "system.settings";
    public const string SystemLogs = "system.logs";
    public const string SystemMonitor = "system.monitor";
}
