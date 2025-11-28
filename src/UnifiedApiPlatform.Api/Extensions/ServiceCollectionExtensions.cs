using FastEndpoints;
using FastEndpoints.Swagger;
using UnifiedApiPlatform.Application;
using UnifiedApiPlatform.Infrastructure;

namespace UnifiedApiPlatform.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // 添加 FastEndpoints
        services.AddFastEndpoints(options =>
        {
            options.DisableAutoDiscovery = false;
        });

        // 添加 Swagger (FastEndpoints 版本)
        services.SwaggerDocument(options =>
        {
            options.DocumentSettings = settings =>
            {
                settings.Title = "UnifiedApiPlatform API";
                settings.Version = "v1";
                settings.Description = "基于 .NET 9.0 的多租户 RBAC 企业级 API 平台";
            };

            // 配置 JWT 认证
            options.EnableJWTBearerAuth = true;
        });

        // 添加 CORS
        var corsSettings = configuration.GetSection("Cors");
        var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // 添加 Application 层服务（稍后创建）
        services.AddApplication();

        // 添加 Infrastructure 层服务
        services.AddInfrastructure(configuration);
        return services;
    }
}
