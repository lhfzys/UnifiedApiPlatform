namespace UnifiedApiPlatform.Api.Endpoints.Roles;

public class GetRolesRequest
{
    /// <summary>
    /// 页码（从 1 开始）
    /// </summary>
    public int PageNumber { get; set; } = 1;

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
    public bool IsDescending { get; set; }

    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 是否系统角色
    /// </summary>
    public bool? IsSystemRole { get; set; }
}
