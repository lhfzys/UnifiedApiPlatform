using System.Text.Json;
using FastEndpoints;
using FluentValidation;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Shared.Constants;
using UnifiedApiPlatform.Shared.Helpers;
using UnifiedApiPlatform.Shared.Models;
using UnifiedApiPlatform.Shared.Resources;

namespace UnifiedApiPlatform.Api.PreProcessors;

/// <summary>
/// FluentValidation 自动验证预处理器
/// 自动执行所有注册的验证器
/// </summary>
public class ValidationPreProcessor<TRequest> : IPreProcessor<TRequest>
{

    public async Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
    {
        // 从请求作用域解析验证器
        var validatorType = typeof(IValidator<>).MakeGenericType(typeof(TRequest));
        var validator = ctx.HttpContext.RequestServices.GetService(validatorType) as IValidator;

        if (validator == null)
        {
            // 没有验证器，跳过
            return;
        }

        // 执行验证
        var context = new FluentValidation.ValidationContext<TRequest>(ctx.Request);
        var result = await validator.ValidateAsync(context, ct);

        if (!result.IsValid)
        {
            // 添加验证错误到 FastEndpoints 的验证失败列表
            foreach (var error in result.Errors)
            {
                ctx.ValidationFailures.Add(new(error.PropertyName, error.ErrorMessage));
            }

            // 自动返回验证错误响应
            if (!ctx.HttpContext.ResponseStarted())
            {
                // 构建验证错误字典
                var validationErrors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                // 使用扩展方法发送错误响应
                await ctx.HttpContext.SendErrorResponseAsync(
                    ErrorCodes.ValidationError,
                    validationErrors,
                    400,
                    ct
                );
            }
        }
    }
}
