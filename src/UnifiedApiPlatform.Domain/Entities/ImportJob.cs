using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 导入任务
/// </summary>
public class ImportJob : MultiTenantEntity
{
    public Guid UserId { get; set; }
    public string JobType { get; set; } = null!; // User/Order/Product
    public string FileName { get; set; } = null!;
    public Guid FileId { get; set; }
    public ImportJobStatus Status { get; set; } = ImportJobStatus.Pending;
    public int TotalRows { get; set; }
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public string? ErrorMessage { get; set; }
    public Instant? StartedAt { get; set; }
    public Instant? CompletedAt { get; set; }

    // 导航属性
    public ICollection<ImportJobDetail> Details { get; set; } = new List<ImportJobDetail>();
}
