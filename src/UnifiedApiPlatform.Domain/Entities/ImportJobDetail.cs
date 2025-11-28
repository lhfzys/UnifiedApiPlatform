using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 导入任务明细
/// </summary>
public class ImportJobDetail : BaseEntity
{
    public Guid JobId { get; set; }
    public ImportJob Job { get; set; } = null!;

    public int RowNumber { get; set; }
    public string? Data { get; set; } // JSON，原始数据
    public string Status { get; set; } = null!; // Success/Failed/Skipped
    public string? ErrorMessage { get; set; }
    public string? CreatedEntityId { get; set; }
    public Instant CreatedAt { get; set; }
}
