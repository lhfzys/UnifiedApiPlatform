using MediatR;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Common.Models;
using UnifiedApiPlatform.Application.Features.Users.Queries.GetUsers;
using UnifiedApiPlatform.Domain.Enums;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

/// <summary>
/// 获取用户列表端点
/// </summary>
public class GetUsersEndpoint(IMediator mediator)
    : QueryEndpointBase<GetUsersRequest, GetUsersQuery, PagedResult<UserDto>>(mediator)
{
    public override void Configure()
    {
        Get("users");
        Options(x => x.WithMetadata(new PermissionMetadata
        {
            Permissions = [Shared.Constants.Policies.UsersView], RequireAll = false
        }));
        Summary(s =>
        {
            s.Summary = "获取用户列表";
            s.Description = "分页获取用户列表，支持搜索、筛选和排序";
            s.ExampleRequest = new GetUsersRequest()
            {
                PageIndex = 1,
                PageSize = 10,
                SortBy = "Id",
                SortDescending = false,
                SearchKeyword = "admin",
                Status = UserStatus.Active,
                IsActive = true,
                OrganizationId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
        });
    }
}
