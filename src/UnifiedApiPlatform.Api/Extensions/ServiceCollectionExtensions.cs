using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;
using UnifiedApiPlatform.Application;
using UnifiedApiPlatform.Infrastructure;

namespace UnifiedApiPlatform.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {

        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Jwt:SecretKey 未在配置中定义");
        }

        services.AddAuthenticationJwtBearer(s =>
            {
                s.SigningKey = secretKey!;
            },
            b =>
            {
                b.TokenValidationParameters.ValidIssuer = issuer;
                b.TokenValidationParameters.ValidAudience = audience;
                b.TokenValidationParameters.ValidateIssuerSigningKey = true;
                b.TokenValidationParameters.ValidateAudience = true;
                b.TokenValidationParameters.ValidateLifetime = true;
                b.TokenValidationParameters.ValidateIssuer = true;
                b.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            });

        services.AddAuthorization();
        // 添加 FastEndpoints
        services.AddFastEndpoints();

        // 添加 Swagger (FastEndpoints 版本)
        services.SwaggerDocument(options =>
        {
            options.EnableJWTBearerAuth = true;
            options.DocumentSettings = settings =>
            {
                settings.Title = "UnifiedApiPlatform API";
                settings.Version = "v1";
                settings.Description = "基于 .NET 9.0 的多租户 RBAC 企业级 API 平台";

                // 添加 JWT Bearer 认证
                settings.AddAuth("Bearer", new()
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Description = "输入 JWT Token，格式：Bearer {token}"
                });
            };

            options.ShortSchemaNames = true;
            options.AutoTagPathSegmentIndex = 1;
        });

        // 添加 CORS
        var corsSettings = configuration.GetSection("Cors");
        var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
                else
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            });
        });

        // 注册业务层与基础设施层
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}
