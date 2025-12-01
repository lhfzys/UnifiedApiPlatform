namespace UnifiedApiPlatform.Application.Common.Queries;

/// <summary>
/// 查询请求标记接口
/// 用于预处理器识别查询类型
/// </summary>
public interface IQueryRequest
{
    string? CurrentUserId { get; set; }
    string? CurrentTenantId { get; set; }
    string? TraceId { get; set; }
}
