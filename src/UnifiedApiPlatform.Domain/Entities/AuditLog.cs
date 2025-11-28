using NodaTime;
using UnifiedApiPlatform.Domain.Common;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 审计日志
/// </summary>
public class AuditLog : BaseEntity
{
    public string TenantId { get; set; } = null!;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // 操作信息
    public AuditAction Action { get; set; }
    public string EntityType { get; set; } = null!;
    public string? EntityId { get; set; }
    public string? EntityName { get; set; }

    // 变更详情
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? ChangedProperties { get; set; } // 逗号分隔

    // 请求信息
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestBody { get; set; }

    // 追踪信息
    public string? TraceId { get; set; }
    public Instant Timestamp { get; set; }
    public int DurationMs { get; set; }

    // 结果
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
