namespace UnifiedApiPlatform.Shared.Helpers;

/// <summary>
/// 文化/语言辅助类
/// </summary>
public static class CultureHelper
{
/// <summary>
    /// 从 Accept-Language 头部解析首选语言
    /// </summary>
    /// <param name="acceptLanguage">Accept-Language 头部值，例如: "zh-CN,zh;q=0.9,en;q=0.8"</param>
    /// <param name="defaultCulture">默认语言</param>
    /// <returns>解析后的语言代码</returns>
    public static string ParseAcceptLanguage(string? acceptLanguage, string defaultCulture = "zh-CN")
    {
        if (string.IsNullOrWhiteSpace(acceptLanguage))
            return defaultCulture;

        // 解析格式: "zh-CN,zh;q=0.9,en;q=0.8"
        var languages = acceptLanguage
            .Split(',')
            .Select(lang =>
            {
                var parts = lang.Split(';');
                var culture = parts[0].Trim();
                var quality = 1.0; // 默认权重

                // 解析 q 值
                if (parts.Length > 1)
                {
                    var qPart = parts[1].Trim();
                    if (qPart.StartsWith("q=") && double.TryParse(qPart[2..], out var q))
                    {
                        quality = q;
                    }
                }

                return new { Culture = culture, Quality = quality };
            })
            .OrderByDescending(x => x.Quality)
            .ToList();

        // 返回第一个支持的语言
        foreach (var lang in languages)
        {
            var culture = NormalizeCulture(lang.Culture);
            if (IsSupportedCulture(culture))
            {
                return culture;
            }
        }

        return defaultCulture;
    }

    /// <summary>
    /// 标准化语言代码
    /// </summary>
    private static string NormalizeCulture(string culture)
    {
        culture = culture.Trim().ToLower();

        // 映射常见的语言代码
        return culture switch
        {
            "zh" or "zh-cn" or "zh-hans" => "zh-CN",
            "zh-tw" or "zh-hk" or "zh-hant" => "zh-TW",
            "en" or "en-us" => "en-US",
            "en-gb" => "en-GB",
            _ => culture
        };
    }

    /// <summary>
    /// 检查是否支持该语言
    /// </summary>
    private static bool IsSupportedCulture(string culture)
    {
        var supportedCultures = new[] { "zh-CN", "zh-TW", "en-US", "en-GB" };
        return supportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 从 HttpContext 获取当前语言
    /// </summary>
    public static string GetCurrentCulture(Microsoft.AspNetCore.Http.HttpContext context)
    {
        var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
        return ParseAcceptLanguage(acceptLanguage);
    }
}
