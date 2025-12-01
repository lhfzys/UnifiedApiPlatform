using System.Text.RegularExpressions;

namespace UnifiedApiPlatform.Shared.Helpers;

/// <summary>
/// 正则表达式帮助类
/// </summary>
public static partial class RegexHelper
{
    /// <summary>
    /// 验证邮箱
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    public static partial Regex EmailRegex();

    /// <summary>
    /// 验证手机号（中国大陆）
    /// </summary>
    [GeneratedRegex(@"^1[3-9]\d{9}$")]
    public static partial Regex PhoneRegex();

    /// <summary>
    /// 验证身份证号（中国大陆）
    /// </summary>
    [GeneratedRegex(@"^[1-9]\d{5}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$")]
    public static partial Regex IdCardRegex();

    /// <summary>
    /// 验证 URL
    /// </summary>
    [GeneratedRegex(@"^https?://[^\s/$.?#].[^\s]*$")]
    public static partial Regex UrlRegex();

    /// <summary>
    /// 验证 IPv4
    /// </summary>
    [GeneratedRegex(@"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$")]
    public static partial Regex IPv4Regex();
}
