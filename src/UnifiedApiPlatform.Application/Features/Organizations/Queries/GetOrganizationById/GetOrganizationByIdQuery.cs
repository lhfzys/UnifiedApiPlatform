using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationById;

public class GetOrganizationByIdQuery: QueryBase<OrganizationDetailDto>
{
    public Guid OrganizationId { get; set; }
}
