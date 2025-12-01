using System.ComponentModel;
using System.Reflection;

namespace UnifiedApiPlatform.Shared.Extensions;

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举的 Description 特性值
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
            return value.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// 枚举转字典（用于下拉列表）
    /// </summary>
    public static Dictionary<int, string> ToDictionary<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .ToDictionary(e => Convert.ToInt32(e), e => e.GetDescription());
    }
}
