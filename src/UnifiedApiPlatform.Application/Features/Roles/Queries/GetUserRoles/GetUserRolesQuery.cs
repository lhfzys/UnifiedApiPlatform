using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetUserRoles;

public class GetUserRolesQuery: QueryBase<List<UserRoleDto>>
{
    public Guid UserId { get; set; }
}
