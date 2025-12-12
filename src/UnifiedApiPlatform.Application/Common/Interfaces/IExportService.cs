namespace UnifiedApiPlatform.Application.Common.Interfaces;

/// <summary>
/// 导出服务接口
/// </summary>
public interface IExportService
{
    /// <summary>
    /// 导出为 Excel
    /// </summary>
    Task<byte[]> ExportToExcelAsync<T>(
        List<T> data,
        string sheetName,
        Dictionary<string, string> columnMappings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出为 CSV
    /// </summary>
    Task<byte[]> ExportToCsvAsync<T>(
        List<T> data,
        Dictionary<string, string> columnMappings,
        CancellationToken cancellationToken = default);
}
