using NodaTime;

namespace UnifiedApiPlatform.Shared.Extensions;

/// <summary>
/// 日期时间扩展方法（基于 NodaTime）
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// 转换为 UTC Instant
    /// </summary>
    public static Instant ToInstant(this DateTime dateTime)
    {
        return Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
    }

    /// <summary>
    /// 从 Instant 转换为 DateTime (UTC)
    /// </summary>
    public static DateTime ToDateTimeUtc(this Instant instant)
    {
        return instant.ToDateTimeUtc();
    }

    /// <summary>
    /// 转换到指定时区
    /// </summary>
    public static ZonedDateTime ToZone(this Instant instant, string timeZoneId)
    {
        var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
        return instant.InZone(zone);
    }

    /// <summary>
    /// 获取时间戳（毫秒）
    /// </summary>
    public static long ToUnixTimeMilliseconds(this Instant instant)
    {
        return instant.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 从时间戳创建 Instant
    /// </summary>
    public static Instant FromUnixTimeMilliseconds(long milliseconds)
    {
        return Instant.FromUnixTimeMilliseconds(milliseconds);
    }

    /// <summary>
    /// 判断是否在指定日期范围内
    /// </summary>
    public static bool IsBetween(this Instant instant, Instant start, Instant end)
    {
        return instant >= start && instant <= end;
    }

    /// <summary>
    /// 格式化为友好时间（如：2小时前）
    /// </summary>
    public static string ToFriendlyString(this Instant instant, IClock? clock = null)
    {
        clock ??= SystemClock.Instance;
        var now = clock.GetCurrentInstant();
        var duration = now - instant;

        if (duration.TotalDays >= 365)
            return $"{(int)(duration.TotalDays / 365)}年前";
        if (duration.TotalDays >= 30)
            return $"{(int)(duration.TotalDays / 30)}个月前";
        if (duration.TotalDays >= 1)
            return $"{(int)duration.TotalDays}天前";
        if (duration.TotalHours >= 1)
            return $"{(int)duration.TotalHours}小时前";
        if (duration.TotalMinutes >= 1)
            return $"{(int)duration.TotalMinutes}分钟前";

        return "刚刚";
    }
}
