using FluentResults;
using MediatR;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Common.Queries;

namespace UnifiedApiPlatform.Application.Features.Roles.Queries.GetRoles;

/// <summary>
/// 获取角色列表查询
/// </summary>
public class GetRolesQuery : PagedQueryBase<RoleDto>
{
    /// <summary>
    /// 搜索关键字（角色名称、显示名称）
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// 是否系统角色
    /// </summary>
    public bool? IsSystemRole { get; set; }
}
