using NodaTime;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 操作日志
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// 租户 ID
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// 用户 ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作类型（Create/Update/Delete/Query/Login/Logout）
    /// </summary>
    public string Action { get; set; } = null!;

    /// <summary>
    /// 实体类型（User/Role/Organization/Menu 等）
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// 实体 ID
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// HTTP 方法（GET/POST/PUT/DELETE）
    /// </summary>
    public string HttpMethod { get; set; } = null!;

    /// <summary>
    /// 请求路径
    /// </summary>
    public string RequestPath { get; set; } = null!;

    /// <summary>
    /// 请求参数（JSON）
    /// </summary>
    public string? RequestBody { get; set; }

    /// <summary>
    /// 响应状态码
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// 响应结果（JSON，失败时记录）
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// IP 地址
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// User-Agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 异常信息（如果有）
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    // 导航属性
    public User? User { get; set; }
}
