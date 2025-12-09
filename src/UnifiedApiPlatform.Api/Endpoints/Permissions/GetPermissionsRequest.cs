namespace UnifiedApiPlatform.Api.Endpoints.Permissions;

public class GetPermissionsRequest
{
    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public string? Category { get; set; }
}
