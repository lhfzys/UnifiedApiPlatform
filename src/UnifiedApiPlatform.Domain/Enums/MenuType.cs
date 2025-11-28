namespace UnifiedApiPlatform.Domain.Enums;

/// <summary>
/// 菜单类型
/// </summary>
public enum MenuType
{
    /// <summary>
    /// 目录（不可点击）
    /// </summary>
    Directory = 0,

    /// <summary>
    /// 菜单（可点击，对应页面）
    /// </summary>
    Menu = 1,

    /// <summary>
    /// 按钮（页面内操作按钮）
    /// </summary>
    Button = 2
}
