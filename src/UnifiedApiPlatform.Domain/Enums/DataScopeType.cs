namespace UnifiedApiPlatform.Domain.Enums;

/// <summary>
/// 数据权限范围类型
/// </summary>
public enum DataScopeType
{
    /// <summary>
    /// 全部数据
    /// </summary>
    All = 1,

    /// <summary>
    /// 自定义（通过表达式或SQL）
    /// </summary>
    Custom = 2,

    /// <summary>
    /// 本组织
    /// </summary>
    Organization = 3,

    /// <summary>
    /// 本组织及子组织
    /// </summary>
    OrganizationAndSub = 4,

    /// <summary>
    /// 仅本人
    /// </summary>
    Self = 5,

    /// <summary>
    /// 指定用户
    /// </summary>
    SpecificUsers = 6
}
