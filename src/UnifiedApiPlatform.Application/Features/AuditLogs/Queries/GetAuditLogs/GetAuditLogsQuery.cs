using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetAuditLogs;

/// <summary>
/// 查询操作日志
/// </summary>
public class GetAuditLogsQuery : QueryBase<PagedResult<AuditLogDto>>
{
    /// <summary>
    /// 用户名（模糊查询）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// 实体类型
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// HTTP 方法
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// 请求路径（模糊查询）
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool? IsSuccess { get; set; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// IP 地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? OrderBy { get; set; } = "CreatedAt";

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool IsDescending { get; set; } = true;
}

/// <summary>
/// 操作日志 DTO
/// </summary>
public class AuditLogDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = null!;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string HttpMethod { get; set; } = null!;
    public string RequestPath { get; set; } = null!;
    public string? RequestBody { get; set; }
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public long Duration { get; set; }
    public string IpAddress { get; set; } = null!;
    public string? UserAgent { get; set; }
    public string? Exception { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime CreatedAt { get; set; }
}
