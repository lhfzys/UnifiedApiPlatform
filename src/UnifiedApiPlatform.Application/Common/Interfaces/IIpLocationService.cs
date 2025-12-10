namespace UnifiedApiPlatform.Application.Common.Interfaces;

/// <summary>
/// IP 地址定位服务接口
/// </summary>
public interface IIpLocationService
{
    /// <summary>
    /// 根据 IP 地址获取地理位置
    /// </summary>
    /// <param name="ipAddress">IP 地址</param>
    /// <returns>地理位置信息（国家|区域|省份|城市|ISP）</returns>
    string? GetLocation(string ipAddress);

    /// <summary>
    /// 根据 IP 地址获取格式化的地理位置
    /// </summary>
    /// <param name="ipAddress">IP 地址</param>
    /// <returns>格式化的地理位置（如：中国-广东省-深圳市）</returns>
    string? GetFormattedLocation(string ipAddress);
}
