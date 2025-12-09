using UnifiedApiPlatform.Application.Common.Queries;
using UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizations;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationTree;

public class GetOrganizationTreeQuery : QueryBase<List<OrganizationTreeNodeDto>>
{
    public bool IncludeInactive { get; set; }
}
