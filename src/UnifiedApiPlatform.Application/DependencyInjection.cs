using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace UnifiedApiPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        // MediatR
        services.AddMediatR(config =>
        {
            config.LicenseKey =
                "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzk0NTI4MDAwIiwiaWF0IjoiMTc2MzAyMzEzNSIsImFjY291bnRfaWQiOiIwMTlhN2M1Y2RjMzY3NzkyODZjMjk4NGYwZDhhZjVkOCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazl5NXc5MGV0OTVxczNqNTNxN2VjNDNmIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.V56_9_8CoaquWCdxwLsSYG2HCuKhNrNzIgEptALu9Dep4Ba5_kIWNpBjzBbGOmhyTs0V8Hb6SNPRapRX6HZgs8yWsZgs6XgZRI5tgVbQWb8SIbxsW1S9qDF9XO-RvKaF1zM0F3iIj98c3ComDZmOlX8ya8tkbromIcw7w84y7__jWjZJt7P8rTk_bOGGms9Rd3C443EwfTIPDvhsLJgPHIg-4r0qSyB-xvOAP3Ay3nKa7MQvIAmvOky8nSMVPq8o4hQvGKDdKUh3YgGFvg18mS1-QFTAtxpIzwo7vSQyryehYwOQqJnYgEf1CO6Jo0qaKS5f70QlrbUQsGQrbiFBYw";

            config.RegisterServicesFromAssembly(assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Mapster
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(assembly);
        services.AddSingleton(mappingConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
