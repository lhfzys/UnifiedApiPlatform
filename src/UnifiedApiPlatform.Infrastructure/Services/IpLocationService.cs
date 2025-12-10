using IP2Region.Net.Abstractions;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Infrastructure.Services;

/// <summary>
/// IP 地址定位服务实现
/// </summary>
public class IpLocationService : IIpLocationService
{
    private readonly ISearcher _searcher;
    private readonly ILogger<IpLocationService> _logger;

    public IpLocationService(ISearcher searcher, ILogger<IpLocationService> logger)
    {
        _searcher = searcher;
        _logger = logger;
    }

    public string? GetLocation(string ipAddress)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return null;

            // 跳过本地 IP
            if (IsLocalIpAddress(ipAddress))
                return "本地";

            // 查询 IP 地理位置
            var result = _searcher.Search(ipAddress);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "IP 地址定位失败: {IpAddress}", ipAddress);
            return null;
        }
    }

    public string? GetFormattedLocation(string ipAddress)
    {
        try
        {
            var location = GetLocation(ipAddress);

            if (string.IsNullOrWhiteSpace(location))
                return null;

            // ip2region 返回格式：国家|区域|省份|城市|ISP
            // 例如：中国|0|广东省|深圳市|电信
            var parts = location.Split('|');

            if (parts.Length < 5)
                return location;

            var country = parts[0];
            var province = parts[2];
            var city = parts[3];

            // 格式化输出
            var formattedParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(country) && country != "0")
                formattedParts.Add(country);

            if (!string.IsNullOrWhiteSpace(province) && province != "0")
                formattedParts.Add(province);

            if (!string.IsNullOrWhiteSpace(city) && city != "0")
                formattedParts.Add(city);

            return formattedParts.Count > 0
                ? string.Join("-", formattedParts)
                : null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "格式化 IP 地理位置失败: {IpAddress}", ipAddress);
            return null;
        }
    }

    /// <summary>
    /// 判断是否为本地 IP 地址
    /// </summary>
    private static bool IsLocalIpAddress(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return true;

        // 常见的本地 IP
        var localIps = new[] { "127.0.0.1", "localhost", "::1", "0.0.0.0" };

        if (localIps.Contains(ipAddress.ToLower()))
            return true;

        // 私有 IP 段
        if (ipAddress.StartsWith("192.168.") ||
            ipAddress.StartsWith("10.") ||
            ipAddress.StartsWith("172.16.") ||
            ipAddress.StartsWith("172.17.") ||
            ipAddress.StartsWith("172.18.") ||
            ipAddress.StartsWith("172.19.") ||
            ipAddress.StartsWith("172.20.") ||
            ipAddress.StartsWith("172.21.") ||
            ipAddress.StartsWith("172.22.") ||
            ipAddress.StartsWith("172.23.") ||
            ipAddress.StartsWith("172.24.") ||
            ipAddress.StartsWith("172.25.") ||
            ipAddress.StartsWith("172.26.") ||
            ipAddress.StartsWith("172.27.") ||
            ipAddress.StartsWith("172.28.") ||
            ipAddress.StartsWith("172.29.") ||
            ipAddress.StartsWith("172.30.") ||
            ipAddress.StartsWith("172.31."))
        {
            return true;
        }

        return false;
    }
}
