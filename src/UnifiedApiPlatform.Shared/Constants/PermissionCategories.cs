namespace UnifiedApiPlatform.Shared.Constants;

/// <summary>
/// 权限分类常量
/// </summary>
public static class PermissionCategories
{
    /// <summary>用户管理</summary>
    public const string UserManagement = "UserManagement";

    /// <summary>角色管理</summary>
    public const string RoleManagement = "RoleManagement";

    /// <summary>权限管理</summary>
    public const string PermissionManagement = "PermissionManagement";

    /// <summary>租户管理</summary>
    public const string TenantManagement = "TenantManagement";

    /// <summary>组织管理</summary>
    public const string OrganizationManagement = "OrganizationManagement";

    /// <summary>菜单管理</summary>
    public const string MenuManagement = "MenuManagement";

    /// <summary>审计日志</summary>
    public const string AuditLogs = "AuditLogs";

    /// <summary>文件管理</summary>
    public const string FileManagement = "FileManagement";

    /// <summary>内容管理</summary>
    public const string ContentManagement = "ContentManagement";

    /// <summary>数据分析</summary>
    public const string DataAnalysis = "DataAnalysis";

    /// <summary>系统设置</summary>
    public const string SystemSettings = "SystemSettings";

    /// <summary>系统管理</summary>
    public const string SystemManagement = "SystemManagement";

    /// <summary>
    /// 获取分类显示名称
    /// </summary>
    public static string GetDisplayName(string category)
    {
        return category switch
        {
            UserManagement => "用户管理",
            RoleManagement => "角色管理",
            PermissionManagement => "权限管理",
            TenantManagement => "租户管理",
            OrganizationManagement => "组织管理",
            MenuManagement => "菜单管理",
            AuditLogs => "审计日志",
            FileManagement => "文件管理",
            ContentManagement => "内容管理",
            DataAnalysis => "数据分析",
            SystemSettings => "系统设置",
            SystemManagement => "系统管理",
            _ => category
        };
    }

    /// <summary>
    /// 获取所有分类
    /// </summary>
    public static List<CategoryInfo> GetAll()
    {
        return
        [
            new(UserManagement, "用户管理", 1000),
            new(RoleManagement, "角色管理", 2000),
            new(PermissionManagement, "权限管理", 3000),
            new(TenantManagement, "租户管理", 4000),
            new(OrganizationManagement, "组织管理", 5000),
            new(MenuManagement, "菜单管理", 6000),
            new(AuditLogs, "审计日志", 7000),
            new(FileManagement, "文件管理", 8000),
            new(ContentManagement, "内容管理", 9000),
            new(DataAnalysis, "数据分析", 10000),
            new(SystemSettings, "系统设置", 11000),
            new(SystemManagement, "系统管理", 12000)
        ];
    }
}

/// <summary>
/// 分类信息
/// </summary>
public record CategoryInfo(string Code, string Name, int SortOrder);
