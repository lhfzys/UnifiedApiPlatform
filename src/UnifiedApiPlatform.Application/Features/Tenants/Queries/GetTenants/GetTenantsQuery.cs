using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Tenants.Queries.GetTenants;

/// <summary>
/// 查询租户列表
/// </summary>
public class GetTenantsQuery : QueryBase<PagedResult<TenantDto>>
{
    /// <summary>
    /// 搜索关键字（名称或编码）
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 租户编码
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool? IsActive { get; set; }

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
/// 租户 DTO
/// </summary>
public class TenantDto
{
    public string Id { get; set; } = null!;
    public string Identifier  { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 统计信息
    /// </summary>
    public TenantStatistics Statistics { get; set; } = new();
}

/// <summary>
/// 租户统计信息
/// </summary>
public class TenantStatistics
{
    /// <summary>
    /// 用户数量
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// 角色数量
    /// </summary>
    public int RoleCount { get; set; }

    /// <summary>
    /// 组织数量
    /// </summary>
    public int OrganizationCount { get; set; }
}
