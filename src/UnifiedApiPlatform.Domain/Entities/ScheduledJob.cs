using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 定时任务配置
/// </summary>
public class ScheduledJob : BaseEntity
{
    public string? TenantId { get; set; } // 系统任务为 null
    public string Name { get; set; } = null!;
    public string JobType { get; set; } = null!; // 任务类型全限定名
    public string CronExpression { get; set; } = null!;
    public string? Parameters { get; set; } // JSON
    public bool IsEnabled { get; set; } = true;
    public Instant? LastRunAt { get; set; }
    public Instant? NextRunAt { get; set; }
    public string? LastStatus { get; set; } // Success/Failed
    public string? Description { get; set; }
    public string CreatedBy { get; set; } = null!;
    public Instant CreatedAt { get; set; }
}
