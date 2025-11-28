using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace UnifiedApiPlatform.Shared.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 判断字符串是否为空或空白
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 判断字符串是否不为空
    /// </summary>
    public static bool IsNotNullOrWhiteSpace(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 安全的 Trim（处理 null）
    /// </summary>
    public static string SafeTrim(this string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 转为驼峰命名
    /// </summary>
    public static string ToCamelCase(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// 转为帕斯卡命名
    /// </summary>
    public static string ToPascalCase(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// 计算 SHA256 哈希
    /// </summary>
    public static string ToSha256(this string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// 脱敏手机号
    /// </summary>
    public static string MaskPhone(this string phone)
    {
        if (phone.IsNullOrWhiteSpace() || phone.Length < 11)
            return phone;

        return Regex.Replace(phone, @"(\d{3})\d{4}(\d{4})", "$1****$2");
    }

    /// <summary>
    /// 脱敏邮箱
    /// </summary>
    public static string MaskEmail(this string email)
    {
        if (email.IsNullOrWhiteSpace() || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        if (parts[0].Length <= 2)
            return email;

        var masked = parts[0][..1] + "***" + parts[0][^1..];
        return $"{masked}@{parts[1]}";
    }

    /// <summary>
    /// 脱敏身份证号
    /// </summary>
    public static string MaskIdCard(this string idCard)
    {
        if (idCard.IsNullOrWhiteSpace() || idCard.Length < 10)
            return idCard;

        return Regex.Replace(idCard, @"(?<=\w{6})\w(?=\w{4})", "*");
    }

    /// <summary>
    /// 截断字符串
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (value.IsNullOrWhiteSpace() || value.Length <= maxLength)
            return value;

        return value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    /// 移除 HTML 标签
    /// </summary>
    public static string StripHtml(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        return Regex.Replace(value, "<.*?>", string.Empty);
    }
}
