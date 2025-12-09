using UnifiedApiPlatform.Shared.Models;

namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 系统权限定义
/// </summary>
public static class PermissionDefinitions
{
    /// <summary>
    /// 所有权限定义
    /// </summary>
    public static readonly List<PermissionDefinition> All =
    [
        // ==================== 系统管理 ====================
        new(PermissionCodes.SystemSettings, "系统设置", PermissionCategories.SystemManagement,
            "配置系统参数和选项", 1000),
        new(PermissionCodes.SystemLogs, "系统日志", PermissionCategories.SystemManagement,
            "查看系统操作日志", 1001),
        new(PermissionCodes.SystemMonitor, "系统监控", PermissionCategories.SystemManagement,
            "监控系统运行状态", 1002),

        // ==================== 用户管理 ====================
        new(PermissionCodes.UsersView, "查看用户", PermissionCategories.UserManagement,
            "查看用户列表和详情", 2000),
        new(PermissionCodes.UsersCreate, "创建用户", PermissionCategories.UserManagement,
            "创建新用户", 2001),
        new(PermissionCodes.UsersUpdate, "更新用户", PermissionCategories.UserManagement,
            "修改用户信息", 2002),
        new(PermissionCodes.UsersDelete, "删除用户", PermissionCategories.UserManagement,
            "删除用户", 2003),

        // ==================== 角色管理 ====================
        new(PermissionCodes.RolesView, "查看角色", PermissionCategories.RoleManagement,
            "查看角色列表和详情", 3000),
        new(PermissionCodes.RolesCreate, "创建角色", PermissionCategories.RoleManagement,
            "创建新角色", 3001),
        new(PermissionCodes.RolesUpdate, "更新角色", PermissionCategories.RoleManagement,
            "修改角色信息", 3002),
        new(PermissionCodes.RolesDelete, "删除角色", PermissionCategories.RoleManagement,
            "删除角色", 3003),

        // ==================== 权限管理 ====================
        new(PermissionCodes.PermissionsView, "查看权限", PermissionCategories.PermissionManagement,
            "查看系统权限列表", 4000),

        // ==================== 组织管理 ====================
        new(PermissionCodes.OrganizationsView, "查看组织", PermissionCategories.OrganizationManagement,
            "查看组织架构", 5000),
        new(PermissionCodes.OrganizationsCreate, "创建组织", PermissionCategories.OrganizationManagement,
            "创建新组织", 5001),
        new(PermissionCodes.OrganizationsUpdate, "更新组织", PermissionCategories.OrganizationManagement,
            "修改组织信息", 5002),
        new(PermissionCodes.OrganizationsDelete, "删除组织", PermissionCategories.OrganizationManagement,
            "删除组织", 5003),

        // ==================== 菜单管理 ====================
        new(PermissionCodes.MenusView, "查看菜单", PermissionCategories.MenuManagement,
            "查看菜单配置", 6000),
        new(PermissionCodes.MenusCreate, "创建菜单", PermissionCategories.MenuManagement,
            "创建新菜单", 6001),
        new(PermissionCodes.MenusUpdate, "更新菜单", PermissionCategories.MenuManagement,
            "修改菜单配置", 6002),
        new(PermissionCodes.MenusDelete, "删除菜单", PermissionCategories.MenuManagement,
            "删除菜单", 6003),

        // ==================== 文件管理 ====================
        new(PermissionCodes.FilesView, "查看文件", PermissionCategories.FileManagement,
            "查看文件列表", 7000),
        new(PermissionCodes.FilesUpload, "上传文件", PermissionCategories.FileManagement,
            "上传新文件", 7001),
        new(PermissionCodes.FilesDelete, "删除文件", PermissionCategories.FileManagement,
            "删除文件", 7002),
    ];

    /// <summary>
    /// 按分类获取权限
    /// </summary>
    public static List<PermissionDefinition> GetByCategory(string category)
    {
        return All.Where(p => p.Category == category)
            .OrderBy(p => p.SortOrder)
            .ToList();
    }

    /// <summary>
    /// 获取所有分类
    /// </summary>
    public static List<string> GetAllCategories()
    {
        return All.Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }
}
