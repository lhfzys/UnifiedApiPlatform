using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 操作日志（API 调用记录）
/// </summary>
public class OperationLog : BaseEntity
{
    public string TenantId { get; set; } = null!;
    public string? UserId { get; set; }
    public string? UserName { get; set; }

    // 请求信息
    public string RequestPath { get; set; } = null!;
    public string RequestMethod { get; set; } = null!;
    public string? RequestHeaders { get; set; } // JSON
    public string? RequestBody { get; set; }
    public string? RequestQuery { get; set; }

    // 响应信息
    public int ResponseStatus { get; set; }
    public string? ResponseBody { get; set; }
    public int ResponseTimeMs { get; set; }

    // 客户端信息
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // 追踪
    public string? TraceId { get; set; }
    public Instant Timestamp { get; set; }

    // 错误信息
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
}
