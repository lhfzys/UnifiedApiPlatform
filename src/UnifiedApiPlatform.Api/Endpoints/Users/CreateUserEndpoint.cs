using MediatR;
using UnifiedApiPlatform.Application.Features.Users.Commands.CreateUser;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class CreateUserEndpoint : CommandEndpointBase<CreateUserRequest, CreateUserCommand, CreateUserResponse>
{
    public CreateUserEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("users");
        Permissions(PermissionCodes.UsersCreate);
        Summary(s =>
        {
            s.Summary = "创建用户";
            s.Description = "创建新用户，需要 users.create 权限";
        });
    }
}
