using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using UnifiedApiPlatform.Application;
using UnifiedApiPlatform.Infrastructure;

namespace UnifiedApiPlatform.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApiServices(
        this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        // 配置 CORS
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // 添加 Application 层服务
        services.AddApplication();

        // 添加 Infrastructure 层服务
        services.AddInfrastructure(configuration);

        // FastEndpoints
        services.AddFastEndpoints();

        var jwtSettings = configuration.GetSection("Jwt");
        services.AddAuthenticationJwtBearer(s =>
        {
            s.SigningKey = jwtSettings["SecretKey"]!;
        },
        b =>
        {
            b.TokenValidationParameters.ValidIssuer = jwtSettings["Issuer"];
            b.TokenValidationParameters.ValidAudience = jwtSettings["Audience"];
            b.TokenValidationParameters.ValidateIssuerSigningKey = true;
            b.TokenValidationParameters.ValidateAudience = true;
            b.TokenValidationParameters.ValidateLifetime = true;
            b.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            b.TokenValidationParameters.ValidateIssuer = true;
        });

        services.AddAuthorization();

        services.SwaggerDocument(options =>
        {
            options.DocumentSettings = s =>
            {
                s.Title = "UnifiedApiPlatform API";
                s.Version = "v1";
                s.Description = "统一 API 平台";
            };
            options.EnableJWTBearerAuth = true;
        });

        return builder;
    }
}
