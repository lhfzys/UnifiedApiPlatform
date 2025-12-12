using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Queries.GetLoginLogs;

/// <summary>
/// 查询登录日志
/// </summary>
public class GetLoginLogsQuery : QueryBase<PagedResult<LoginLogDto>>
{
    /// <summary>
    /// 用户名（模糊查询）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 登录类型
    /// </summary>
    public string? LoginType { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool? IsSuccess { get; set; }

    /// <summary>
    /// IP 地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 地理位置
    /// </summary>
    public string? Location { get; set; }

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
/// 登录日志 DTO
/// </summary>
public class LoginLogDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string LoginType { get; set; } = null!;
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
    public string IpAddress { get; set; } = null!;
    public string? UserAgent { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public string? DeviceType { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}
