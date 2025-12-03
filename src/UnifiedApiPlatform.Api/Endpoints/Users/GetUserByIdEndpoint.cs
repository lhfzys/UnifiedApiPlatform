using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Features.Users.Queries.GetUserById;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class GetUserByIdEndpoint(IMediator mediator) : Endpoint<GetUserByIdRequest>
{
    public override void Configure()
    {
        Get("users/{id}");
        Options(x => x.WithMetadata(new PermissionMetadata
        {
            Permissions = [Shared.Constants.Policies.UsersView], RequireAll = false
        }));
        Summary(s =>
        {
            s.Summary = "获取用户详情";
            s.Description = "根据用户 ID 获取用户的详细信息";
        });

        Description(b => b
            .Produces<UserDetailDto>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404));
    }

    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        var currentUser = HttpContext.RequestServices
            .GetRequiredService<ICurrentUserService>();

        var query = new GetUserByIdQuery
        {
            UserId = req.Id,
            CurrentUserId = currentUser.UserId,
            CurrentTenantId = currentUser.TenantId,
            TraceId = HttpContext.TraceIdentifier
        };

        var result = await mediator.Send(query, ct);

        // 使用统一响应扩展
        await this.SendResultAsync(result, ct);
    }
}
