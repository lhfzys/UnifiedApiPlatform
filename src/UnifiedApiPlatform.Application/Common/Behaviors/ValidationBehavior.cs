using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace UnifiedApiPlatform.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        _logger.LogWarning(
            "验证失败: {RequestType}, 错误数量: {ErrorCount}",
            typeof(TRequest).Name,
            failures.Count);

        var errorsByField = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray()
            );

        foreach (var (field, errors) in errorsByField)
        {
            _logger.LogWarning("  - {Field}: {Errors}", field, string.Join("; ", errors));
        }

        var response = new TResponse();

        var validationError = new Error("VALIDATION_ERROR")
            .WithMetadata("ValidationErrors", errorsByField);

        response.Reasons.Add(validationError);

        return response;
    }
}
