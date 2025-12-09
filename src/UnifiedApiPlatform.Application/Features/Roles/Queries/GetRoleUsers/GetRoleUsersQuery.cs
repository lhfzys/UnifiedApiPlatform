using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoleUsers;

public class GetRoleUsersQuery: PagedQueryBase<RoleUserDto>
{
    public Guid RoleId { get; set; }
    public string? SearchKeyword { get; set; }
    public bool? IsActive { get; set; }
}
