namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 授权策略常量
/// </summary>
public static class PermissionCodes
{
     // ==================== 用户管理 ====================
    public const string UsersView = "Users.View";
    public const string UsersCreate = "Users.Create";
    public const string UsersUpdate = "Users.Update";
    public const string UsersDelete = "Users.Delete";
    public const string UsersResetPassword = "Users.ResetPassword";
    public const string UsersActivate = "Users.Activate";
    public const string UsersDeactivate = "Users.Deactivate";
    public const string UsersExport = "Users.Export";
    public const string UsersImport = "Users.Import";

    // ==================== 角色管理 ====================
    public const string RolesView = "Roles.View";
    public const string RolesCreate = "Roles.Create";
    public const string RolesUpdate = "Roles.Update";
    public const string RolesDelete = "Roles.Delete";
    public const string RolesAssignPermissions = "Roles.AssignPermissions";

    // ==================== 权限管理 ====================
    public const string PermissionsView = "Permissions.View";
    public const string PermissionsCreate = "Permissions.Create";
    public const string PermissionsUpdate = "Permissions.Update";
    public const string PermissionsDelete = "Permissions.Delete";

    // ==================== 租户管理 ====================
    public const string TenantsView = "Tenants.View";
    public const string TenantsCreate = "Tenants.Create";
    public const string TenantsUpdate = "Tenants.Update";
    public const string TenantsDelete = "Tenants.Delete";

    // ==================== 组织管理 ====================
    public const string OrganizationsView = "Organizations.View";
    public const string OrganizationsCreate = "Organizations.Create";
    public const string OrganizationsUpdate = "Organizations.Update";
    public const string OrganizationsDelete = "Organizations.Delete";

    // ==================== 菜单管理 ====================
    public const string MenusView = "Menus.View";
    public const string MenusCreate = "Menus.Create";
    public const string MenusUpdate = "Menus.Update";
    public const string MenusDelete = "Menus.Delete";

    // ==================== 审计日志 ====================
    public const string AuditLogsView = "AuditLogs.View";
    public const string AuditLogsExport = "AuditLogs.Export";
    public const string AuditLogsDelete = "AuditLogs.Delete";
    public const string AuditLogsStatistics = "AuditLogs.Statistics";

    // ==================== 文件管理 ====================
    public const string FilesView = "Files.View";
    public const string FilesUpload = "Files.Upload";
    public const string FilesDownload = "Files.Download";
    public const string FilesDelete = "Files.Delete";

    // ==================== 系统设置 ====================
    public const string SystemSettingsView = "SystemSettings.View";
    public const string SystemSettingsUpdate = "SystemSettings.Update";

    // ==================== 系统管理 ====================
    public const string SystemBackup = "System.Backup";
    public const string SystemRestore = "System.Restore";
    public const string SystemLogs = "System.Logs";
}
