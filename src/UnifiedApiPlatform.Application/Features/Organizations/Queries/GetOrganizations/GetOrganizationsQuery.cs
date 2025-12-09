using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizations;

/// <summary>
/// 获取组织列表查询
/// </summary>
public class GetOrganizationsQuery: QueryBase<List<OrganizationDto>>
{
    public string? SearchKeyword { get; set; }
    public Guid? ParentId { get; set; }
    public bool? IsActive { get; set; }
    public bool AsTree { get; set; }
}
