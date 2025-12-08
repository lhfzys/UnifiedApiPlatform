using MediatR;
using UnifiedApiPlatform.Application.Features.Users.Commands.ChangePassword;

namespace UnifiedApiPlatform.Api.Endpoints.Users;

public class ChangePasswordEndpoint : CommandEndpointBase<ChangePasswordRequest, ChangePasswordCommand>
{
    public ChangePasswordEndpoint(IMediator mediator) : base(mediator)
    {
    }

    public override void Configure()
    {
        Post("users/change-password");

        // 修改自己的密码不需要特殊权限（已认证即可）
        // 但如果要重置他人密码，需要 users.update 权限
        // 这里我们允许已认证用户访问，在 Handler 中再判断
        Summary(s =>
        {
            s.Summary = "修改密码";
            s.Description = "修改自己的密码（需要原密码）或管理员重置他人密码（需要 users.update 权限）";
        });
    }
}
