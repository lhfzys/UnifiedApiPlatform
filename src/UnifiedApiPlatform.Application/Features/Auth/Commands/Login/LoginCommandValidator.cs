using FluentValidation;

namespace UnifiedApiPlatform.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(255).WithMessage("邮箱长度不能超过255个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于6个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过100个字符");
    }
}
