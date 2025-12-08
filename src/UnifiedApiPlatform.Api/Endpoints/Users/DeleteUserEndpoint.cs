using MediatR;
using UnifiedApiPlatform.Application.Features.Users.Commands.DeleteUser;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class DeleteUserEndpoint : CommandEndpointBase<DeleteUserRequest, DeleteUserCommand>
{
    public DeleteUserEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Delete("users/{userId}");
        Permissions(PermissionCodes.UsersDelete);
        Summary(s =>
        {
            s.Summary = "删除用户";
            s.Description = "软删除用户，不会物理删除数据";
        });
    }
}
