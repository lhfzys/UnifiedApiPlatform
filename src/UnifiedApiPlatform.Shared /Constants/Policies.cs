namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 授权策略常量
/// </summary>
public static class Policies
{
    // 角色策略
    public const string SuperAdmin = "SuperAdmin";
    public const string TenantAdmin = "TenantAdmin";

    // 租户管理
    public const string TenantsView = "tenants.view";
    public const string TenantsCreate = "tenants.create";
    public const string TenantsUpdate = "tenants.update";
    public const string TenantsDelete = "tenants.delete";

    // 用户管理
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";
    public const string UsersResetPassword = "users.resetPassword";
    public const string UsersExport = "users.export";
    public const string UsersImport = "users.import";

    // 角色管理
    public const string RolesView = "roles.view";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";
    public const string RolesAssignPermissions = "roles.assignPermissions";
    public const string RolesAssignMenus = "roles.assignMenus";

    // 权限管理
    public const string PermissionsView = "permissions.view";
    public const string PermissionsCreate = "permissions.create";
    public const string PermissionsUpdate = "permissions.update";
    public const string PermissionsDelete = "permissions.delete";

    // 菜单管理
    public const string MenusView = "menus.view";
    public const string MenusCreate = "menus.create";
    public const string MenusUpdate = "menus.update";
    public const string MenusDelete = "menus.delete";

    // 组织管理
    public const string OrganizationsView = "organizations.view";
    public const string OrganizationsCreate = "organizations.create";
    public const string OrganizationsUpdate = "organizations.update";
    public const string OrganizationsDelete = "organizations.delete";

    // 字典管理
    public const string DictionariesView = "dictionaries.view";
    public const string DictionariesCreate = "dictionaries.create";
    public const string DictionariesUpdate = "dictionaries.update";
    public const string DictionariesDelete = "dictionaries.delete";

    // 审计日志
    public const string AuditLogsView = "auditLogs.view";
    public const string AuditLogsExport = "auditLogs.export";

    // 公告管理
    public const string AnnouncementsView = "announcements.view";
    public const string AnnouncementsCreate = "announcements.create";
    public const string AnnouncementsUpdate = "announcements.update";
    public const string AnnouncementsDelete = "announcements.delete";

    // 系统监控
    public const string MonitoringView = "monitoring.view";
    public const string OnlineUsersView = "onlineUsers.view";
}
