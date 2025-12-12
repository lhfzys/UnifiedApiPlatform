namespace UnifiedApiPlatform.Application.Common.Models;

/// <summary>
/// 导出文件结果
/// </summary>
public class ExportFileResult
{
    public byte[] FileContent { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
