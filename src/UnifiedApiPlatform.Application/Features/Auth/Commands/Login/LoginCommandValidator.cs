using FluentValidation;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Account)
            .NotEmpty().WithMessage("请输入用户名或邮箱");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("请输入密码");
    }
}
