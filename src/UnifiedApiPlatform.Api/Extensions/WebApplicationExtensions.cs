using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UnifiedApiPlatform.Api.PreProcessors;
using UnifiedApiPlatform.Infrastructure.Middleware;
using UnifiedApiPlatform.Infrastructure.Persistence;
using UnifiedApiPlatform.Infrastructure.Persistence.Seeds;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        // 全局异常处理
        app.UseMiddleware<GlobalExceptionMiddleware>();
        // Serilog 请求日志
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            };
        });

        // CORS
        app.UseCors("DefaultCorsPolicy");

        // HTTPS 重定向
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        // 认证和授权
        app.UseAuthentication();
        app.UseAuthorization();

        // FastEndpoints
        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
            config.Security.PermissionsClaimType = CustomClaimTypes.Permission;
            config.Endpoints.Configurator = ep =>
            {
                ep.PreProcessors(
                    Order.Before,
                    typeof(ValidationPreProcessor<>),
                    typeof(PermissionAuthorizationPreProcessor<>)
                );
            };
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerGen(uiConfig: c =>
            {
                c.Path = "/swagger";
                c.DefaultModelsExpandDepth = -1;
            });
        }

        return app;
    }
}
