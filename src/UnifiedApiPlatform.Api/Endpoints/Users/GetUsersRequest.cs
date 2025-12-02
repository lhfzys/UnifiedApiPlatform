using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class GetUsersRequest
{
    /// <summary>
    /// 页码（从 1 开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool SortDescending { get; set; } = false;

    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 用户状态筛选
    /// </summary>
    public UserStatus? Status { get; set; }

    /// <summary>
    /// 是否激活筛选
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// 组织 ID 筛选
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// 角色 ID 筛选
    /// </summary>
    public Guid? RoleId { get; set; }
}
