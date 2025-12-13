using FluentValidation;

namespace UnifiedApiPlatform.Application.Features.Tenants.Commands.CreateTenant;

/// <summary>
/// 创建租户命令验证器
/// </summary>
public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("租户标识不能为空")
            .MaximumLength(50).WithMessage("租户标识长度不能超过 50 个字符")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("租户标识只能包含字母、数字、下划线和连字符");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("租户名称不能为空")
            .MaximumLength(200).WithMessage("租户名称长度不能超过 200 个字符");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述长度不能超过 500 个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ContactName)
            .MaximumLength(100).WithMessage("联系人姓名长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.ContactName));

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("联系人邮箱格式不正确")
            .MaximumLength(100).WithMessage("联系人邮箱长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));

        RuleFor(x => x.ContactPhone)
            .MaximumLength(50).WithMessage("联系人电话长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(x.ContactPhone));

        RuleFor(x => x.MaxUsers)
            .GreaterThan(0).WithMessage("最大用户数必须大于 0")
            .LessThanOrEqualTo(100000).WithMessage("最大用户数不能超过 100000");

        RuleFor(x => x.MaxStorageInGB)
            .GreaterThan(0).WithMessage("最大存储空间必须大于 0")
            .LessThanOrEqualTo(10000).WithMessage("最大存储空间不能超过 10000 GB");

        RuleFor(x => x.MaxApiCallsPerDay)
            .GreaterThan(0).WithMessage("每日最大 API 调用次数必须大于 0")
            .LessThanOrEqualTo(10000000).WithMessage("每日最大 API 调用次数不能超过 10000000");
    }
}
