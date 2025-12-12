using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Tenants.Queries.GetTenantById;

/// <summary>
/// 根据 ID 查询租户
/// </summary>
public class GetTenantByIdQuery : QueryBase<TenantDetailDto>
{
    public string Id { get; set; } = null!;
}

/// <summary>
/// 租户详情 DTO
/// </summary>
public class TenantDetailDto
{
    public string Id { get; set; } = null!;
    public string Identifier { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 详细统计信息
    /// </summary>
    public TenantDetailStatistics Statistics { get; set; } = new();
}

/// <summary>
/// 租户详细统计
/// </summary>
public class TenantDetailStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalRoles { get; set; }
    public int TotalOrganizations { get; set; }
    public int TotalMenus { get; set; }
    public DateTime? LastLoginTime { get; set; }
}
